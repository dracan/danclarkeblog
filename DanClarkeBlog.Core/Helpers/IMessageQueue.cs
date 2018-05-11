using System;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IMessageQueue : IDisposable
    {
        void Send(string queueMessage, string message);
        void Subscribe(string queueMessage, Func<string, Task> callbackAsync);
    }
}
