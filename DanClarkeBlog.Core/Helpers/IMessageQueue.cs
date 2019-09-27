using System;
using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IMessageQueue : IDisposable
    {
        Task SendAsync(string queueMessage, string message, CancellationToken cancellationToken);
        Task SubscribeAsync(string queueMessage, Func<string, Task> callbackAsync, CancellationToken cancellationToken);
    }
}
