using EventStore.ClientAPI;

namespace GiacenzaCQRS.Core.Events
{
    public interface IMessage 
    {
        void Process<TView>(TView view) where TView : IGiacenzaProjection;
        void ProcessV2<TView2>(TView2 view, int version, string originalStreamId) where TView2 : IGiacenzaProjectionV2;
        void ProcessV3<TView3>(TView3 view, Position position) where TView3 : IGiacenzaProjectionV3;
    }
}