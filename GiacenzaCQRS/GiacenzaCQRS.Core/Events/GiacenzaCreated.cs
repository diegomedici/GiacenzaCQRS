using System;
using System.Runtime.Serialization;
using GiacenzaCQRS.Core.Projections;

namespace GiacenzaCQRS.Core.Events
{
    [DataContract]
    public class GiacenzaCreated : IMessage
    {
        [DataMember]
        public string Minsan { get; private set; }
        [DataMember]
        public Guid Id { get; private set; }

        public GiacenzaCreated(string minsan, Guid id)
        {
            Minsan = minsan;
            Id = id;
        }

        public void Process<TView>(TView view) where TView : IGiacenzaProjection
        {
            view.When(this);
        }

        public void ProcessV2<TView2>(TView2 view) where TView2 : IGiacenzaProjectionV2
        {
            view.Create(Id, Minsan);
        }
    }
}