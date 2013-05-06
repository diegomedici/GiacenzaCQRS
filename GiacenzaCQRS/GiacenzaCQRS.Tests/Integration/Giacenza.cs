using System;
using System.Collections;
using CommonDomain;

namespace GiacenzaCQRS.Tests.Integration
{
    public class Giacenza : IAggregate
    {
        public Giacenza(string minsan)
        {
            throw new NotImplementedException();
        }

        public void ApplyEvent(object @event)
        {
            throw new NotImplementedException();
        }

        public ICollection GetUncommittedEvents()
        {
            throw new NotImplementedException();
        }

        public void ClearUncommittedEvents()
        {
            throw new NotImplementedException();
        }

        public IMemento GetSnapshot()
        {
            throw new NotImplementedException();
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }
    }
}