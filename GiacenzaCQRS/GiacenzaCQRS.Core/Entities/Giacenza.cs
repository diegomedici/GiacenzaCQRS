using System;
using CommonDomain;
using CommonDomain.Core;
using GiacenzaCQRS.Core.Events;

namespace GiacenzaCQRS.Core.Entities
{
    public class Giacenza : AggregateBase
    {
        protected string Minsan { get; private set; }

        public Giacenza(string minsan, IRouteEvents routeEvents) : base(routeEvents)
        {
            if (string.IsNullOrEmpty(minsan) || minsan.Length != 9) throw new ArgumentException("minsan");
            Register<GiacenzaCreated>(e =>
                {
                    Id = e.Id;
                    Minsan = e.Minsan;
                });
            RaiseEvent(new GiacenzaCreated(minsan, Guid.NewGuid()));
         
        }

        public Giacenza(string minsan) : this(minsan, null)
        {
            
        }

        protected Giacenza()
        {
            Register<GiacenzaCreated>(e =>
            {
                Id = e.Id;
                Minsan = e.Minsan;
            });
        }
        

    }
}