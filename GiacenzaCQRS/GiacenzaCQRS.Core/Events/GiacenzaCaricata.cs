using System;

namespace GiacenzaCQRS.Core.Events
{
    public class GiacenzaCaricata
    {
        public int Quantita { get; private set; }
        public Guid Id { get; private set; }

        public GiacenzaCaricata(Guid id, int quantita)
        {
            Id = id;
            Quantita = quantita;
        }
    }
}