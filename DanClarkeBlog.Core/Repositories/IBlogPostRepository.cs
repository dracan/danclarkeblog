using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Repositories
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken);
        Task<BlogPostListing> GetAllAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetUpdatesAsync(string cursor, CancellationToken cancellationToken);
        Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetWithConditionAsync(Func<BlogPost, bool> conditionFunc, CancellationToken cancellationToken);
        Task AddAsync(BlogPost post, CancellationToken cancellationToken);
        Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken);
        Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken);
        Task<List<BlogPost>> GetRecentAsync(int numRecent, CancellationToken cancellationToken);

        // Tags
        Task<List<TagCount>> GetTagCountsAsync(CancellationToken cancellationToken);

        Task SetDropboxCursorAsync(string cursor, CancellationToken cancellationToken);
        Task<string> GetDropboxCursorAsync(CancellationToken cancellationToken);
    }
}