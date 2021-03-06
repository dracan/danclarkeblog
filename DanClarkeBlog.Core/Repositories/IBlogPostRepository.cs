using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Repositories
{
    public interface IBlogPostSourceRepository { }
    public interface IBlogPostTargetRepository { }

    public interface IBlogPostRepository : IBlogPostSourceRepository, IBlogPostTargetRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync(CursorContainer cursor, CancellationToken cancellationToken);
        Task<BlogPostListing> GetPublishedAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken);
        Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken);
        Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken);
        Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken);
        Task<List<BlogPost>> GetRecentAsync(int numRecent, CancellationToken cancellationToken);
        Task<BlogPost> GetDraftByIdAsync(Guid draftId, CancellationToken cancellationToken);
        Task<BlogPost> GetPublishedByRouteAsync(string route, CancellationToken cancellationToken);

        // Tags
        Task<List<TagCount>> GetTagCountsAsync(CancellationToken cancellationToken);
        Task RemoveUnusedTagsAsync(CancellationToken cancellationToken);

        Task SetDropboxCursorAsync(string cursor, CancellationToken cancellationToken);
        Task<string> GetDropboxCursorAsync(CancellationToken cancellationToken);
    }
}