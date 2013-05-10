using System;
using System.Runtime.Serialization;
using EventStore.ClientAPI;

namespace GiacenzaCQRS.Core.Events
{
    [DataContract]
    public class GiacenzaUpdated : IMessage
    {
        public GiacenzaUpdated(string minsan, Guid id, int quantita)
        {
            
            Minsan = minsan;
            Id = id;
            NuovaGiacenza = quantita;
        }

        [DataMember]
        protected Guid Id { get; set; }
        [DataMember]
        public int NuovaGiacenza { get; set; }
        [DataMember]
        private string Minsan { get; set; }

        public void Process<TView>(TView view) where TView : IGiacenzaProjection
        {
            throw new System.NotImplementedException();
        }

        public void ProcessV2<TView2>(TView2 view, int version, string originalStreamId) where TView2 : IGiacenzaProjectionV2
        {
            view.Aggiorna(Minsan, NuovaGiacenza, version, originalStreamId);
        }

        public void ProcessV3<TView3>(TView3 view, Position position) where TView3 : IGiacenzaProjectionV3
        {
            view.Aggiorna(Minsan, NuovaGiacenza, position);
        }
    }
}