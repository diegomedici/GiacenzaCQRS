using System;
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
            IEventStoreConnection connection = EventStoreConnection.Create(IntegrationTestTcpEndPoint);
            connection.Connect();
            //var eventSincProjectionMaker = new EventSincProjectionMaker(connection,
            //                                                            new GiacenzaProjection(
            //                                                                new FileDocumentReaderWriter
            //                                                                    <string, GiacenzaReadModel>("c:\\Pippo", new DocumentStrategy())));

            string connString = @"Data Source=TIMIDO;Initial Catalog=GIACENZE-CQRS;User ID=sa;Password=sensazioniforti";
            string ravendDbConnString = @"http://aleb-sfbs:8080";
            

            var eventSincProjectionMaker = new EventSinkProjectionMaker(connection, new GiacenzaProjectionV3(ravendDbConnString));

            eventSincProjectionMaker.GoLive();
            Console.ReadLine();
        }
    }


}
