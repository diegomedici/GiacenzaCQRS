using System;
using GiacenzaCQRS.Core.Events;

namespace GiacenzaCQRS.Core.ReadModels
{
    public class GiacenzaReadModel
    {
        
        public Guid Id { get; set; }
        public string Minsan { get; set; }
        public int Quantita { get; set; }
    }
}