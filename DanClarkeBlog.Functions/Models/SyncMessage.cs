using JetBrains.Annotations;

namespace DanClarkeBlog.Functions.Models
{
    [UsedImplicitly]
    public class SyncMessage
    {
        public bool IsIncremental { get; set; }
    }
}