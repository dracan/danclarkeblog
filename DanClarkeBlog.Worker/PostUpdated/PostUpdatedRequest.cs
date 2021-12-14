using DanClarkeBlog.Worker.Models;
using MediatR;

namespace DanClarkeBlog.Worker.PostUpdated;

public class PostUpdatedRequest : IRequest
{
    public PostUpdatedRequest(SyncMessage message)
    {
        Message = message;
    }

    public SyncMessage Message { get; init; }
}
