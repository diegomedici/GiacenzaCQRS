using System;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Entities;
using Raven.Client.Document;

namespace GiacenzaCQRS.Core.Events
{
    public class GiacenzaProjectionV3 : IGiacenzaProjectionV3
    {
        private GlobalStreamPosition _globalStreamPosition;
        public Position GlobalPosition
        {
            get { return new Position(_globalStreamPosition.CommmitPosition, _globalStreamPosition.PreparePosition); }
        }

     

        private readonly DocumentStore _documentStore;


        public GiacenzaProjectionV3(string connection)
        {
            _documentStore = new DocumentStore {Url = connection, DefaultDatabase = "GiacenzeCQRS"};
            _documentStore.Initialize();
            using (var session = _documentStore.OpenSession())
            {
                var position = session.Load<GlobalStreamPosition>("GLOBAL");
                if (position == null)
                {
                    _globalStreamPosition = new GlobalStreamPosition {CommmitPosition = 0, PreparePosition = 0, Id = "GLOBAL"};
                    session.Store(_globalStreamPosition);
                    session.SaveChanges();

                }
                else _globalStreamPosition = position;
            }

        }

        public void Create(Guid id, string minsan, Position position)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new GiacenzaView
                    {
                        GiacenzaId = id,
                        Id = minsan,
                        Quantita = 0
                    });
                _globalStreamPosition.CommmitPosition = position.CommitPosition;
                _globalStreamPosition.PreparePosition = position.PreparePosition;
                session.Store(_globalStreamPosition);
                session.SaveChanges();
            }
        }

        public GiacenzaView Read(string minsan)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<GiacenzaView>(minsan);
            }
        }

        public void Aggiorna(string minsan, int quantita, Position position)
        {
            using (var session = _documentStore.OpenSession())
            {
                var giacenza = session.Load<GiacenzaView>(minsan);
                giacenza.Quantita = quantita;
                _globalStreamPosition.CommmitPosition = position.CommitPosition;
                _globalStreamPosition.PreparePosition = position.PreparePosition;
                session.Store(_globalStreamPosition);
                session.SaveChanges();
            }
        }
    }
}