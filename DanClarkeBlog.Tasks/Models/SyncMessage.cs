using JetBrains.Annotations;

namespace DanClarkeBlog.Tasks.Models
{
    [UsedImplicitly]
    public class SyncMessage
    {
        public bool IsIncremental { get; set; }
    }
}