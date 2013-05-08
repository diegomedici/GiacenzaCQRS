namespace GiacenzaCQRS.Core.Events
{
    public interface IGiacenzaProjection
    {
        void When(GiacenzaCreated e);
        void When(GiacenzaCaricata e);
        void When(GiacenzaScaricata e);
    }
}