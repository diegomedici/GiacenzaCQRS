namespace GiacenzaCQRS.Core.Events
{
    public interface IMessage 
    {
        void Process<TView>(TView view) where TView : IGiacenzaProjection;
        void ProcessV2<TView2>(TView2 view, int version, string originalStreamId) where TView2 : IGiacenzaProjectionV2;
    }
}