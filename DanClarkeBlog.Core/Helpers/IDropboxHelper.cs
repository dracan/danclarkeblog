using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IDropboxHelper
    {
        Task<List<string>> GetFilesAsync(string path, CancellationToken cancellationToken);
        Task<List<string>> GetFilesAsync(string path, string cursor, CancellationToken cancellationToken);
        Task<byte[]> GetFileContentAsync(string path, CancellationToken cancellationToken);
    }
}