﻿using System;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Events;

namespace Giacenza.TestClient
{
    public class EventSincProjectionMaker
    {
        private readonly EventStoreConnection _connection;
        private readonly IGiacenzaProjection _view;

        public EventSincProjectionMaker(EventStoreConnection connection, IGiacenzaProjection view)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (view == null) throw new ArgumentNullException("view");

            _connection = connection;
            _view = view;
        }

        public void GoLive()
        {
            var posizione = Position.Start;
            _connection.SubscribeToAllFrom(posizione, true, DispatchToHandler);
        }

        private void DispatchToHandler(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            var e = GetEventStoreHelpers.DeserializeEvent(resolvedEvent.OriginalEvent.Metadata,
                                                          resolvedEvent.OriginalEvent.Data) as IMessage;
            if (e != null)
            {
                e.Process(_view);
            }



        

            //e.Process();

            //_dispatcher.DispatchToHandlers(e);
            //_subscriptionContextProvider.SetLastProcessedPositionFor(_inputStream, resolvedEvent.OriginalEventNumber);
        }
    }
}