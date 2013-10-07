using System;
using System.Collections.Generic;
using GiacenzaCQRS.Core.Events;
using GiacenzaCQRS.Core.ReadModels;
using GiacenzaCQRS.Core.Storage;

namespace GiacenzaCQRS.Core.Projections
{

    public class GiacenzaProjection : IGiacenzaProjection
    {
        private readonly IDocumentWriter<string, GiacenzaReadModel> _store;

        public GiacenzaProjection(IDocumentWriter<string, GiacenzaReadModel> store)
        {
            if(store == null) throw new ArgumentNullException("store");
            _store = store;
        }

        public void When(GiacenzaCreated e)
        {
            if(e == null) throw new ArgumentNullException("e");
            GiacenzaReadModel giacenza = _store.AddOrUpdate(e.Minsan, () => new GiacenzaReadModel
                {
                    Id = e.Id,
                    Minsan = e.Minsan,
                    Quantita = 0,
                }, model => model);
        }

        public void When(GiacenzaUpdated e)
        {
            if (e == null) throw new ArgumentNullException("e");
            //GiacenzaReadModel giacenza = _store.AddOrUpdate(e.)
        }
    }
}