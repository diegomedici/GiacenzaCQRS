using System;

namespace GiacenzaCQRS.Core.Events
{
    public class GiacenzaCreated
    {
        public string Minsan { get; private set; }
        public Guid Id { get; private set; }

        public GiacenzaCreated(string minsan, Guid id)
        {
            Minsan = minsan;
            Id = id;
        }
    }
}