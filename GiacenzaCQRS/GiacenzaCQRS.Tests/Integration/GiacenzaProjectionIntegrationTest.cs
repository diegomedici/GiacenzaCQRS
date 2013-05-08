using System;
using System.IO;
using GiacenzaCQRS.Core.Events;
using GiacenzaCQRS.Core.Projections;
using GiacenzaCQRS.Core.ReadModels;
using GiacenzaCQRS.Core.Storage;
using NUnit.Framework;

namespace GiacenzaCQRS.Core.Entities
{
    [TestFixture]
    public class GiacenzaProjectionIntegrationTest
    {
        [Test]
        public void GiacenzaProjection_GiacenzaCreated()
        {
            FileDocumentReaderWriter<string, GiacenzaReadModel> fileDocument = new FileDocumentReaderWriter<string, GiacenzaReadModel>("c:\\pippo", new DocumentStrategy());
            GiacenzaProjection giacenzaProjection = new GiacenzaProjection(fileDocument);
            string minsan = "123456789";
            giacenzaProjection.When(new GiacenzaCreated(minsan, Guid.NewGuid()));
            
            GiacenzaReadModel read;
            fileDocument.TryGet(minsan, out read);

            Assert.IsNotNull(read);
            Assert.AreEqual("123456789", read.Minsan);
            Assert.AreEqual(0, read.Quantita);

            giacenzaProjection.When(new GiacenzaCaricata(minsan, 100));

            fileDocument.TryGet(minsan, out read);

            Assert.IsNotNull(read);
            Assert.AreEqual("123456789", read.Minsan);
            Assert.AreEqual(100, read.Quantita);


            giacenzaProjection.When(new GiacenzaScaricata(minsan, 10));

            fileDocument.TryGet(minsan, out read);

            Assert.IsNotNull(read);
            Assert.AreEqual("123456789", read.Minsan);
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