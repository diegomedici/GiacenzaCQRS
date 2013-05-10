using System;
using System.Collections.Generic;
using GiacenzaCQRS.Core.Entities;
using Raven.Client.Document;

namespace GiacenzaCQRS.Core.Events
{
    public class RavendDbProjection : IGiacenzaProjectionV2
    {
        
        private Dictionary<string, int> _streamLevels = new Dictionary<string, int>();
        private DocumentStore _documentStore;

        public RavendDbProjection(string connection)
        {
            _documentStore = new DocumentStore { Url = connection, DefaultDatabase = "GiacenzeCQRS" };
            _documentStore.Initialize();

        }

        private int GetStreamVersion(string streamId)
        {
            if (_streamLevels.ContainsKey(streamId)) return _streamLevels[streamId];
            _streamLevels[streamId] = GetLastSavedVersion(streamId);
            return _streamLevels[streamId];
        }


        private int GetLastSavedVersion(string streamId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var doc = session.Load<StreamPosition>(streamId);
                if (doc == null)
                {
                    session.Store(new StreamPosition
                    {
                        Id = streamId,
                        Position = 0
                    });
                    session.SaveChanges();
                    return 0;
                }
                return doc.Position;
            }
                     
        }


        public void Create(Guid id, string minsan, int versione, string originalStreamId)
        {
            if (GetStreamVersion(originalStreamId) < versione)
            {
                using (var session = _documentStore.OpenSession())
                {
                    session.Store(new GiacenzaView
                        {
                            GiacenzaId = id,
                            Id = minsan,
                            Quantita = 0
                        });
                    session.Store(
                        new StreamPosition
                            {
                                Id = originalStreamId,
                                Position = versione
                            });
                    session.SaveChanges();
                }
            }

        }

        public void Carica(string minsan, int quantita, int versione)
        {
            throw new NotImplementedException();
        }

        public void Scarica(string minsan, int quantita, int versione)
        {
            throw new NotImplementedException();
        }

        public void Aggiorna(string minsan, int quantita, int versione, string originalStreamId)
        {
            if (GetStreamVersion(originalStreamId) < versione)
            {
                using (var session = _documentStore.OpenSession())
                {
                    var view = session.Load<GiacenzaView>(minsan);
                    view.Quantita = quantita;
                    var stream = session.Load<StreamPosition>(originalStreamId);
                    stream.Position = versione;
                    session.SaveChanges();
                }
            }
        }
    }
}