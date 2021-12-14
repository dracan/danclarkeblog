using JetBrains.Annotations;

namespace DanClarkeBlog.Worker.Models;

[UsedImplicitly]
public class SyncMessage
{
    public bool IsIncremental { get; set; }
}