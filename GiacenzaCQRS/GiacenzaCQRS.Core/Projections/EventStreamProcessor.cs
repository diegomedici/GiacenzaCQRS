using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Transactions;
using EventStore.ClientAPI;
using ThreadState = System.Threading.ThreadState;
using Timer = System.Timers.Timer;

/*
 * Suggest using this code with a projection like this
 * it does not work with $all
 * 
fromAll().whenAny(
   function(state, event) {
      linkTo('all', event);
      return state;
   }
);
*/


namespace GiacenzaCQRS.Core.Projections
{
    public class EventStreamProcessor
    {

        private const int StatsInterval = 10 * 1000;

        private readonly ConcurrentQueue<RecordedEvent> _catchupQueue = new ConcurrentQueue<RecordedEvent>();
        private readonly ConcurrentQueue<RecordedEvent> _toProcessQueue = new ConcurrentQueue<RecordedEvent>();

        private readonly ILastSeenPositionRepository _lastSeenPositionRepository;
        private readonly IDispatchRecordedEvents _eventDispatcher;
        private readonly EventStoreConnection _eventStoreConnection;
        private readonly string _eventStreamName;

        private int _statisticsCounter; //used for stats

        public EventStreamProcessor(EventStoreConnection eventStoreConnection, string eventStreamName,
                                    IDispatchRecordedEvents eventDispatcher,
                                    ILastSeenPositionRepository lastSeenRepository, bool printStats = false)
        {
            _eventStoreConnection = eventStoreConnection;
            _eventStreamName = eventStreamName;
            _lastSeenPositionRepository = lastSeenRepository;
            _eventDispatcher = eventDispatcher;

            if (!printStats) return;
            //set up a timer to output stats
            var timer = new Timer(StatsInterval);
            timer.Elapsed += (a, b) => PrintStats(StatsInterval);
            timer.Start();
        }

        private void PrintStats(int intervalms)
        {
            Console.WriteLine("\n{0} events processed in the last {1} seconds", _statisticsCounter, intervalms / 1000);
            Console.WriteLine("{0} events/second average", _statisticsCounter / 10);
            Console.WriteLine("{0} events in real time queue, {1} in catch up queue", _toProcessQueue.Count, _catchupQueue.Count);
            _statisticsCounter = 0;
        }

        public void CatchUpAndProcessFrom(int lastSeenPosition)
        {
            var startingLastSeenPosition = lastSeenPosition;

            _eventStoreConnection.SubscribeAsync(_eventStreamName, e => EnqueueEvent(e, _toProcessQueue), ConnectionDropped);


            var eventLoaderThread = new Thread(() => ReadToEndOrFirstEventSeenViaSubscription(startingLastSeenPosition));
            eventLoaderThread.Start();

            Stopwatch catchUpTimer = Stopwatch.StartNew();
            while (!(eventLoaderThread.ThreadState == ThreadState.Stopped && _catchupQueue.IsEmpty))
            {
                try
                {
                    lastSeenPosition = PerformQueueProcessing(_catchupQueue, lastSeenPosition, _toProcessQueue);
                }
                catch (QueueOverlapException ex)
                {
                    //catch up queue has hit the subscription queue, we are done catching up!
                    lastSeenPosition = ex.LastSeenPosition;
                    eventLoaderThread.Abort();
                    break;
                }
            }

            catchUpTimer.Stop();

            var eventsProcessed = lastSeenPosition - startingLastSeenPosition;
            Console.WriteLine("Caught up {0} events in {1} seconds, {2}events/second", eventsProcessed, catchUpTimer.Elapsed.TotalSeconds, eventsProcessed / catchUpTimer.Elapsed.TotalSeconds);

            //begin real time processing forever
            while (true)
                lastSeenPosition = PerformQueueProcessing(_toProcessQueue, lastSeenPosition);
        }

        //returns the last processed position
        private int PerformQueueProcessing(ConcurrentQueue<RecordedEvent> queue, int lastSeenPosition, ConcurrentQueue<RecordedEvent> queueToPeek = null)
        {
            int currentPosition = lastSeenPosition;
            bool queueOverlap = false;
            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (true)
                {
                    RecordedEvent nextEvent;
                    if (!queue.TryDequeue(out nextEvent))
                        break; //empty! break and commit transaction

                    //peeking used is to make sure no overlap when catching up and subscriptions come in
                    if (queueToPeek != null && PeekAndCompare(nextEvent, queueToPeek))
                    {
                        queueOverlap = true;
                        break;
                    }

                    //update counters
                    currentPosition++;
                    _statisticsCounter++;

                    //dispatch the event
                    DispatchIfNotSystemEvent(nextEvent);

                    //break and commit the transaction every 512 or 512ms
                    if (currentPosition % 512 == 0 || sw.ElapsedMilliseconds > 1000)
                        break;
                }

                //no need to commit the transaction if we didnt process any events
                if (currentPosition > lastSeenPosition)
                {
                    //update the last seen position before commiting the transaction
                    _lastSeenPositionRepository.SaveLastSeenPosition(currentPosition);
                    ts.Complete();
                    lastSeenPosition = currentPosition;
                }

            }

            if (queueOverlap)
                throw new QueueOverlapException(lastSeenPosition);

            return lastSeenPosition;
        }

        private void ReadToEndOrFirstEventSeenViaSubscription(int lastSeenEvent)
        {
            var pageNumber = 0;
            const int pageSize = 1000;

            StreamEventsSlice page;
            do
            {
                page = _eventStoreConnection.ReadStreamEventsForward(_eventStreamName,
                                                              lastSeenEvent + (pageNumber*pageSize) + 1, pageSize, false);
                

                foreach (var e in page.Events)
                {
                    if (PeekAndCompare(e, _toProcessQueue))
                        return;

                    EnqueueEvent(e, _catchupQueue);
                }

                pageNumber++;
            } while (page.Events == page.Events.Length);
        }

        private static bool PeekAndCompare(RecordedEvent e, ConcurrentQueue<RecordedEvent> queue)
        {
            RecordedEvent firstOnPeekQueue;
            return (queue.TryPeek(out firstOnPeekQueue) && e.EventId == firstOnPeekQueue.EventId);
        }


        private void ConnectionDropped()
        {
            //TODO: Something here. Reconnect logic?
        }

        private void DispatchIfNotSystemEvent(RecordedEvent recordedEvent)
        {
            if (recordedEvent.EventStreamId.StartsWith("$") || recordedEvent.EventType.StartsWith("$"))
                return;

            _eventDispatcher.Dispatch(recordedEvent);
        }

        private void EnqueueEvent(RecordedEvent recordedEvent, ConcurrentQueue<RecordedEvent> queue)
        {
            //resolve link events client side since there is no better way of dealing with this currently
            if (recordedEvent.EventType.Equals("$>"))
                recordedEvent = ResolveLinkEvent(recordedEvent);

            queue.Enqueue(recordedEvent);
        }

        private RecordedEvent ResolveLinkEvent(RecordedEvent recordedEvent)
        {
            var decodedData = Encoding.UTF8.GetString(recordedEvent.Data);
            var elements = decodedData.Split(new[] { '@' });

            int eventNumber;
            if (elements.Length != 2 || !Int32.TryParse(elements[0], out eventNumber))
                throw new ArgumentException("Couldnt parse link event format:" + decodedData);

            var streamId = elements[1];

            var slice = _eventStoreConnection.ReadEventStreamForward(streamId, eventNumber, 1);

            if (slice.Events.Length != slice.Count)
                throw new StreamDoesNotExistException("Could not get event: " + decodedData);

            return slice.Events[0];
        }
    }

    internal class QueueOverlapException : Exception
    {
        public int LastSeenPosition { get; set; }
        public QueueOverlapException(int lastSeenPosition)
        {
            LastSeenPosition = lastSeenPosition;
        }
    }
}