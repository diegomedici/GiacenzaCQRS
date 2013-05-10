using System;
using System.Runtime.Serialization;
using EventStore.ClientAPI;
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

        public void ProcessV2<TView2>(TView2 view, int version, string originalStreamId) where TView2 : IGiacenzaProjectionV2
        {
            view.Create(Id, Minsan, version, originalStreamId);
        }

        public void ProcessV3<TView3>(TView3 view, Position position) where TView3 : IGiacenzaProjectionV3
        {
            view.Create(Id, Minsan, position);
        }
    }
}