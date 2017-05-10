using System;
using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Repositories
{
    public interface ILockRepository
    {
        Task AcquireLockAsync(string key, int numRetries, TimeSpan lockTimeout, CancellationToken cancellationToken);
        Task ReleaseLockAsync(CancellationToken cancellationToken);
    }
}