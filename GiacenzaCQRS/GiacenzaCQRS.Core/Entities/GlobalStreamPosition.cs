using EventStore.ClientAPI;

namespace GiacenzaCQRS.Core.Entities
{
    public class GlobalStreamPosition
    {
        public string Id { get; set; }
        public long CommmitPosition { get; set; }
        public long PreparePosition { get; set; }
    }
}