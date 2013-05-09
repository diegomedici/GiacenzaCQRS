namespace GiacenzaCQRS.Core.Events
{
    public interface IGiacenzaProjection
    {
        void When(GiacenzaCreated e);
        void When(GiacenzaUpdated e);
    }
}