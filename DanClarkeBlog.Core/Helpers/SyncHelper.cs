using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Repositories;

namespace DanClarkeBlog.Core.Helpers
{
    public class SyncHelper
    {
        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo, IBlogPostRepository destRepo, bool incremental, CancellationToken cancellationToken)
        {
            string dropboxCursor = null;

            if (incremental)
            {
                // Try to get a persisted cursor from our SQL database, if that's null (so we haven't got one),
                // then get the very latest from Dropbox itself.
                dropboxCursor = await destRepo.GetDropboxCursorAsync(cancellationToken) ?? await sourceRepo.GetDropboxCursorAsync(cancellationToken);
            }

            var sourcePosts = incremental
                ? (await sourceRepo.GetUpdatesAsync(dropboxCursor, cancellationToken)).ToList()
                : (await sourceRepo.GetAllAsync(cancellationToken)).ToList();

            var destPosts = await destRepo.GetAllAsync(cancellationToken);

            foreach(var sourcePost in sourcePosts)
            {
                await destRepo.AddOrUpdateAsync(sourcePost, cancellationToken);
            }

            var postsToDelete = destPosts.Where(d => sourcePosts.All(s => s.Title != d.Title));

            await destRepo.DeleteAsync(postsToDelete, cancellationToken);
        }
    }
}