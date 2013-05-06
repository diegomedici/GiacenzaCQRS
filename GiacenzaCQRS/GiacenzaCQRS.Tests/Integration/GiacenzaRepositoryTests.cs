using System;
using CommonDomain;
using CommonDomain.Persistence;
using GiacenzaCQRS.Core.Repositories;
using NUnit.Framework;

namespace GiacenzaCQRS.Core.Entities
{
    [TestFixture]
    public class GiacenzaRepositoryTests
    {

         [Test]
         public void GiacenzaRepository_SaveGiacenzaProdotto123456789_Saved()
         {
             IRepository giacenzaRepository = new GiacenzaRepository();
             IAggregate giacenza = new Giacenza("123456789");
             giacenzaRepository.Save(giacenza, Guid.NewGuid(), objects => { });
             var savedGiacenza = giacenzaRepository.GetById<Giacenza>(giacenza.Id);
             Assert.AreEqual(savedGiacenza.Id, giacenza.Id);

         }
    }
}