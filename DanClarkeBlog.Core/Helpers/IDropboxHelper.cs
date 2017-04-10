using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IDropboxHelper
    {
        Task<List<DropboxFileModel>> GetFilesAsync(string path, CancellationToken cancellationToken);
        Task<List<DropboxFileModel>> GetFilesAsync(string path, CursorContainer cursor, CancellationToken cancellationToken);
        Task<byte[]> GetFileContentAsync(string path, CancellationToken cancellationToken);
    }
}