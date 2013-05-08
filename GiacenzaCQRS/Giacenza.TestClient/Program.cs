using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Events;
using GiacenzaCQRS.Core.Projections;
using GiacenzaCQRS.Core.ReadModels;
using GiacenzaCQRS.Core.Storage;

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
            var eventSincProjectionMaker = new EventSincProjectionMaker(connection, new GiacenzaProjectionV2(connString));

            eventSincProjectionMaker.GoLive();
            Console.ReadLine();
        }
    }


}
