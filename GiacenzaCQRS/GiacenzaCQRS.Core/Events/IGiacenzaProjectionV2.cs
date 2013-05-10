using System;

namespace GiacenzaCQRS.Core.Events
{
    public interface IGiacenzaProjectionV2
    {
        void Create(Guid id, string minsan, int versione, string originalStreamId);
        void Carica(string minsan, int quantita, int versione);
        void Scarica(string minsan, int quantita, int versione);
        void Aggiorna(string minsan, int quantita, int versione, string originalStreamId);
    }
}