using System;

namespace GiacenzaCQRS.Core.Events
{
    [Serializable]
    public class GiacenzaScaricata : IMessage
    {
        public Guid Id { get; private set; }
        public int Quantita { get; private set; }
        public string Minsan { get; private set; }

        public GiacenzaScaricata(string minsan, int quantita)
        {
            Quantita = quantita;
            Minsan = minsan;
        }

        public void Process<TView>(TView view) where TView : IGiacenzaProjection
        {
            view.When(this);
        }

        public void ProcessV2<TView2>(TView2 view) where TView2 : IGiacenzaProjectionV2
        {
            throw new NotImplementedException();
        }
    }
}