﻿using System;
using System.Net;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Events;

namespace Giacenza.TestClient
{
    class Program
    {

        private static readonly IPEndPoint IntegrationTestTcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);

        static void Main(string[] args)
        {
            EventStoreConnection connection = EventStoreConnection.Create();
            connection.Connect(IntegrationTestTcpEndPoint);
            //var eventSincProjectionMaker = new EventSincProjectionMaker(connection,
            //                                                            new GiacenzaProjection(
            //                                                                new FileDocumentReaderWriter
            //                                                                    <string, GiacenzaReadModel>("c:\\Pippo", new DocumentStrategy())));

            string connString = @"Data Source=TIMIDO;Initial Catalog=GIACENZE-CQRS;User ID=sa;Password=sensazioniforti";
            string ravendDbConnString = @"http://aleb-sfbs:8080";
            

            var eventSincProjectionMaker = new EventSinkProjectionMaker(connection, new RavendDbProjection(ravendDbConnString));

            eventSincProjectionMaker.GoLive();
            Console.ReadLine();
        }
    }


}
