using System;
using System.Collections.Generic;
using GiacenzaCQRS.Core.Events;
using GiacenzaCQRS.Core.Projections;
using GiacenzaCQRS.Core.ReadModels;
using GiacenzaCQRS.Core.Storage;
using Moq;
using NUnit.Framework;

namespace GiacenzaCQRS.Tests.Unit.Projections
{
    [TestFixture]
    public class GiacenzaProjectionTests
    {
        private Mock<IDocumentWriter<Guid, GiacenzaReadModel>> _db;
        private Guid _id;
        //private GiacenzaCreated _giacenzaCreated;
        private GiacenzaProjection _giacenzaProjection;

        [SetUp]
        public void Init()
        {
            _db = new Mock<IDocumentWriter<Guid, GiacenzaReadModel>>();
            _id = Guid.NewGuid();
            _giacenzaProjection = new GiacenzaProjection(_db.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _db = null;
            _giacenzaProjection = null;
        }

        [Test]
        public void GiacenzaProjection_ConstructorTests()
        {
            Assert.Throws<ArgumentNullException>(()=> new GiacenzaProjection(null));
        }

        [Test]
        public void GiacenzaProjection_When_GiacenzaCreatedNull_ThrowsArgumentException()
        {
            GiacenzaCreated giacenzaCreated = null;
            Assert.Throws<ArgumentNullException>(() => _giacenzaProjection.When(giacenzaCreated));
        }

        //[Test]
        //public void GiacenzaProjection_When_GiacenzaCreated_Quantita0()
        //{
        //    GiacenzaCreated giacenzaCreated = new GiacenzaCreated("123456789", _id);
        //    _giacenzaProjection.When(giacenzaCreated);
        //    Assert.True(_db.ContainsKey(_id));
        //    GiacenzaReadModel giacenzaRead = _db[_id];
        //    Assert.AreEqual("123456789", giacenzaRead.Minsan);
        //    Assert.AreEqual(0, giacenzaRead.Quantita);
        //}

        //[Test]
        //public void GiacenzaProjection_When_GiacenzaCaricata()
        //{
        //    GiacenzaReadModel giacenzaReadModel = new GiacenzaReadModel();
        //    giacenzaReadModel.Id = _id;
        //    giacenzaReadModel.Minsan = "123456789";
        //    giacenzaReadModel.Quantita = 100;
        //    _db.Add(_id, giacenzaReadModel);

        //    _giacenzaProjection.When(new GiacenzaCaricata(_id, 1));
        //    Assert.AreEqual(101, _db[_id].Quantita);
        //}
         
        //[Test]
        //public void GiacenzaProjection_When_GiacenzaScaricata()
        //{
        //    GiacenzaReadModel giacenzaReadModel = new GiacenzaReadModel();
        //    giacenzaReadModel.Id = _id;
        //    giacenzaReadModel.Minsan = "123456789";
        //    giacenzaReadModel.Quantita = 100;
        //    _db.Add(_id, giacenzaReadModel);

        //    _giacenzaProjection.When(new GiacenzaScaricata(_id, 1));
        //    Assert.AreEqual(99, _db[_id].Quantita);
        //}
         
    }
}