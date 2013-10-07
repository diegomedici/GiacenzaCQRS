using System;
using System.IO;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Events;
using GiacenzaCQRS.Core.Projections;
using GiacenzaCQRS.Core.ReadModels;
using GiacenzaCQRS.Core.Storage;
using NUnit.Framework;

namespace GiacenzaCQRS.Tests.Integration
{
    [TestFixture]
    public class GiacenzaProjectionIntegrationTest
    {
        string ravendDbConnString = @"http://aleb-sfbs:8080";

        [Test]
        public void GiacenzaProjection_GiacenzaCreated()
        {
            
            var giacenzaProjection = new GiacenzaProjectionV3(ravendDbConnString);
            string minsan = "123456789";
            var id = Guid.NewGuid();

            giacenzaProjection.Create(id, minsan, Position.Start);

            var read = giacenzaProjection.Read(minsan);

            Assert.IsNotNull(read);
            Assert.AreEqual("123456789", read.Id);
            Assert.AreEqual(0, read.Quantita);
            Assert.AreEqual(id, read.GiacenzaId);

            giacenzaProjection.Aggiorna(minsan, 100, Position.Start);

            read = giacenzaProjection.Read(minsan);

            Assert.IsNotNull(read);
            Assert.AreEqual("123456789", read.Id);
            Assert.AreEqual(100, read.Quantita);
            Assert.AreEqual(id, read.GiacenzaId);


            giacenzaProjection.Aggiorna(minsan, 90, Position.Start);

            read = giacenzaProjection.Read(minsan);

            Assert.IsNotNull(read);
            Assert.AreEqual("123456789", read.Id);
            Assert.AreEqual(90, read.Quantita);


        }

        [SetUp]
        public void TearDown()
        {
            if (Directory.Exists("c:\\pippo"))
                Directory.Delete("c:\\pippo", true);
        }
         
    }
}