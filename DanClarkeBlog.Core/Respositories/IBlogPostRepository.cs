using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Respositories
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(BlogPost post, CancellationToken cancellationToken);
    }
}