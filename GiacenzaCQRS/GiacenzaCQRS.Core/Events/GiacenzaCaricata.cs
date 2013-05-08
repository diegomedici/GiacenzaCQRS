using System;

namespace GiacenzaCQRS.Core.Events
{
    [Serializable]
    public class GiacenzaCaricata : IMessage
    {
        public int Quantita { get; private set; }
        public Guid Id { get; private set; }

        public string Minsan { get; private set; }

        public GiacenzaCaricata(string minsan, int quantita)
        {
            Minsan = minsan;
            Quantita = quantita;
        }

        //public void Process(IProjection projection)
        //{
        //    projection.When(this);
        //}

        public void Process<TView>(TView view) where TView : IGiacenzaProjection
        {
            view.When(this);
        }
    }
}