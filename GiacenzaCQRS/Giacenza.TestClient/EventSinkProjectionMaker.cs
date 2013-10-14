using System;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Events;

namespace Giacenza.TestClient
{
    public class EventSinkProjectionMaker
    {
        private readonly IEventStoreConnection _connection;
        private readonly IGiacenzaProjection _view;
        private readonly IGiacenzaProjectionV2 _view2;
        private readonly IGiacenzaProjectionV3 _view3;

        public EventSinkProjectionMaker(IEventStoreConnection connection, IGiacenzaProjection view)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (view == null) throw new ArgumentNullException("view");

            _connection = connection;
            _view = view;
        }

        public EventSinkProjectionMaker(IEventStoreConnection connection, IGiacenzaProjectionV2 view)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (view == null) throw new ArgumentNullException("view");

            _connection = connection;
            _view2 = view;
        }

        public EventSinkProjectionMaker(IEventStoreConnection connection, IGiacenzaProjectionV3 view)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (view == null) throw new ArgumentNullException("view");

            _connection = connection;
            _view3 = view;
        }

        public void GoLive()
        {

            _connection.SubscribeToAllFrom(_view3.GlobalPosition, true, DispatchToHandler);

        }

        private void DispatchToHandler(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            var e = GetEventStoreHelpers.DeserializeEvent(resolvedEvent.OriginalEvent.Metadata,
                                                          resolvedEvent.OriginalEvent.Data) as IMessage;
            if (e != null)
            {
                //e.Process(_view);                
                //e.ProcessV2(_view2, resolvedEvent.OriginalEventNumber, resolvedEvent.OriginalStreamId);                
                e.ProcessV3(_view3, resolvedEvent.OriginalPosition ?? Position.Start);

            }



        

            //e.Process();

            //_dispatcher.DispatchToHandlers(e);
            //_subscriptionContextProvider.SetLastProcessedPositionFor(_inputStream, resolvedEvent.OriginalEventNumber);
        }
    }
}