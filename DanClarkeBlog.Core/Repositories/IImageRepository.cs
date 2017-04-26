using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Repositories
{
    public interface IImageRepository
    {
        Task AddAsync(string destPath, string fileName, byte[] data, CancellationToken cancellationToken);
    }
}
