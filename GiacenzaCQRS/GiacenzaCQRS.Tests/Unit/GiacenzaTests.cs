using System;
using CommonDomain;
using GiacenzaCQRS.Core.Entities;
using GiacenzaCQRS.Core.Events;
using Moq;
using NUnit.Framework;

namespace GiacenzaCQRS.Tests.Unit
{
    [TestFixture]
    public class GiacenzaTests
    {
        [Test]
        public void Giacenza_ConstructorTests()
        {
            var mockRoute = new Mock<IRouteEvents>();
            Assert.Throws<ArgumentException>(() => new Giacenza(null, mockRoute.Object));
            Assert.Throws<ArgumentException>(() => new Giacenza(string.Empty, mockRoute.Object));
            Assert.Throws<ArgumentException>(() => new Giacenza("123", mockRoute.Object));            
        }

        [Test]
        public void Giacenza_NewGiacenza_NewGiacenzaEventRaised()
        {
            var mockRoute = new Mock<IRouteEvents>();
            var giacenza = new Giacenza("123456789", mockRoute.Object);
            mockRoute.Verify(r => r.Dispatch(It.IsAny<GiacenzaCreated>()), Times.Once());

        }
    }
}