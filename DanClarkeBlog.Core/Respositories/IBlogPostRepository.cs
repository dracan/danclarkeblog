using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Respositories
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken);
        Task<BlogPostListing> GetAllAsync(int? offset, int? maxResults, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPost>> GetWithConditionAsync(Func<BlogPost, bool> conditionFunc, CancellationToken cancellationToken);
        Task AddAsync(BlogPost post, CancellationToken cancellationToken);
        Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken);
        Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken);
    }
}