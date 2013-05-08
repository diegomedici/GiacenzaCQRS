using System;
using System.Runtime.Serialization;
using GiacenzaCQRS.Core.Events;

namespace GiacenzaCQRS.Core.ReadModels
{
    [DataContract]
    public class GiacenzaReadModel
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Minsan { get; set; }
        [DataMember]
        public int Quantita { get; set; }
    }
}