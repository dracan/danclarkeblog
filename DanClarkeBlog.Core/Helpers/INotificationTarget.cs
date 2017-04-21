using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface INotificationTarget
    {
        Task SendMessageAsync(string message, CancellationToken cancellationToken);
    }
}
