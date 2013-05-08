namespace GiacenzaCQRS.Core.Events
{
    public interface IMessage 
    {
        void Process<TView>(TView view) where TView : IGiacenzaProjection;
    }
}