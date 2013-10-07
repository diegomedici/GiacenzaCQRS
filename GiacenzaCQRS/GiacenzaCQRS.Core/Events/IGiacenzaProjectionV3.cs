using System;
using EventStore.ClientAPI;
using GiacenzaCQRS.Core.Entities;

namespace GiacenzaCQRS.Core.Events
{
    public interface IGiacenzaProjectionV3
    {
        void Create(Guid id, string minsan, Position position);
        void Aggiorna(string minsan, int quantita, Position position);
        Position GlobalPosition { get; }
        GiacenzaView Read(string minsan);
    }
}