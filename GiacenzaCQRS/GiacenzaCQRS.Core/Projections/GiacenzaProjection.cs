using System;
using System.Collections.Generic;
using GiacenzaCQRS.Core.Events;
using GiacenzaCQRS.Core.ReadModels;

namespace GiacenzaCQRS.Core.Projections
{
    public class GiacenzaProjection
    {
        private readonly IDictionary<Guid, GiacenzaReadModel> _db;

        public GiacenzaProjection(IDictionary<Guid, GiacenzaReadModel> db)
        {
            if(db == null) throw new ArgumentNullException("db");
            _db = db;
        }

        public void When(GiacenzaCreated e)
        {
            if(e == null) throw new ArgumentNullException("e");

            GiacenzaReadModel read = new GiacenzaReadModel
                {
                    Id = e.Id,
                    Minsan = e.Minsan,
                    Quantita = 0,
                };
            _db.Add(read.Id, read);
        }

        public void When(GiacenzaCaricata e)
        {
            if (e == null) throw new ArgumentNullException("e");
                var read = _db[e.Id];
                read.Quantita += e.Quantita;
        }

        public void When(GiacenzaScaricata e)
        {
            if (e == null) throw new ArgumentNullException("e");
                var read = _db[e.Id];
                read.Quantita -= e.Quantita;
        }
    }
}