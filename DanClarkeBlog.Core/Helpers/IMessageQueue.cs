using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IMessageQueue
    {
        Task SendAsync(string topicName, string message, CancellationToken cancellationToken);
    }
}
