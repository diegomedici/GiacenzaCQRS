using System;

namespace GiacenzaCQRS.Core.Events
{
    public class GiacenzaScaricata
    {
        public Guid Id { get; private set; }
        public int Quantita { get; private set; }

        public GiacenzaScaricata(Guid id, int quantita)
        {
            Id = id;
            Quantita = quantita;
        }
    }
}