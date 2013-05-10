using System;
using System.Data;
using System.Data.SqlClient;
using CommonDomain;
using CommonDomain.Persistence;
using GiacenzaCQRS.Core.Entities;
using GiacenzaCQRS.Core.Repositories;
using NUnit.Framework;

namespace GiacenzaCQRS.Tests.Integration
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

        
         [Test]
         public void GiacenzaRepository_SaveGiacenzaProdotto123456788_Saved()
         {
             IRepository giacenzaRepository = new GiacenzaRepository();
             IAggregate giacenza = new Giacenza("123456788");
             giacenzaRepository.Save(giacenza, Guid.NewGuid(), objects => { });
             var savedGiacenza = giacenzaRepository.GetById<Giacenza>(giacenza.Id);
             Assert.AreEqual(savedGiacenza.Id, giacenza.Id);

         }

         [Test]
         public void GiacenzaRepository_SaveAndUpdateGiacenzaProdotto123456787_Saved()
         {
             IRepository giacenzaRepository = new GiacenzaRepository();
             IAggregate giacenza = new Giacenza("123456787");
             giacenzaRepository.Save(giacenza, Guid.NewGuid(), objects => { });
             var savedGiacenza = giacenzaRepository.GetById<Giacenza>(giacenza.Id);
             Assert.AreEqual(savedGiacenza.Id, giacenza.Id);
             savedGiacenza.Carica(10);
             giacenzaRepository.Save(savedGiacenza, Guid.NewGuid(), objects => { });
             var nuovaGiacenza = giacenzaRepository.GetById<Giacenza>(savedGiacenza.Id);
             Assert.AreEqual(savedGiacenza.Id, nuovaGiacenza.Id);
             //Assert.AreEqual(10, GetGiacenza("123456787"));

             nuovaGiacenza.Scarica(2);
             giacenzaRepository.Save(nuovaGiacenza, Guid.NewGuid(), objects => { });

             var nuovissimaGiacenza = giacenzaRepository.GetById<Giacenza>(nuovaGiacenza.Id);
             Assert.AreEqual(nuovaGiacenza.Id, nuovissimaGiacenza.Id);
             Assert.AreEqual(8, GetGiacenza("123456787"));


         }

        protected int GetGiacenza(string minsan)
        {
            var getQuantitaCmdString = "select Quantita from giacenzaView where minsan='{0}'";
            string connString = @"Data Source=TIMIDO;Initial Catalog=GIACENZE-CQRS;User ID=sa;Password=sensazioniforti";
            var sqlCommand = new SqlCommand(string.Format(getQuantitaCmdString, minsan));
            sqlCommand.CommandType = CommandType.Text;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                sqlCommand.Connection = conn;
                object o = sqlCommand.ExecuteScalar();
                return Convert.ToInt32(o);
            }
        }


        [SetUp]
        public void Init()
        {
            var updateCmdString = "delete from GiacenzaView";
            string connString = @"Data Source=TIMIDO;Initial Catalog=GIACENZE-CQRS;User ID=sa;Password=sensazioniforti";
            var sqlCommand = new SqlCommand(updateCmdString);
            sqlCommand.CommandType = CommandType.Text;

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                sqlCommand.Connection = conn;
                sqlCommand.ExecuteNonQuery();
            }
        }

    }
}