using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Core.Repositories;

namespace DanClarkeBlog.Core.Helpers
{
    public class SyncHelper
    {
        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo, IBlogPostRepository destRepo, bool incremental, ILogger logger, CancellationToken cancellationToken)
        {
            logger.Trace($"SynchronizeBlogPostsAsync with incremental = {incremental}");

            var dropboxCursor = new CursorContainer();

            if (incremental)
            {
                // Try to get a persisted cursor from our SQL database, if that's null (so we haven't got one), then we'll do a full update
                dropboxCursor.Cursor = await destRepo.GetDropboxCursorAsync(cancellationToken);

                logger.Trace($"Cursor = {dropboxCursor.Cursor}");
            }

            var sourcePosts = incremental && !string.IsNullOrWhiteSpace(dropboxCursor.Cursor)
                ? (await sourceRepo.GetUpdatesAsync(dropboxCursor, cancellationToken)).ToList()
                : (await sourceRepo.GetAllAsync(cancellationToken)).ToList();

            var destPosts = (await destRepo.GetAllAsync(cancellationToken)).ToList();

            logger.Trace($"Processing {sourcePosts.Count} source posts, and {destPosts.Count()} dest posts");

            foreach(var sourcePost in sourcePosts)
            {
                await destRepo.AddOrUpdateAsync(sourcePost, cancellationToken);
            }

            var postsToDelete = destPosts.Where(d => sourcePosts.All(s => s.Title != d.Title)).ToList();

            logger.Trace($"Found {postsToDelete.Count} to delete");

            await destRepo.DeleteAsync(postsToDelete, cancellationToken);

	        if (incremental)
	        {
                logger.Trace($"Saving new Dropbox cursor: {dropboxCursor.Cursor}");

		        await destRepo.SetDropboxCursorAsync(dropboxCursor.Cursor, cancellationToken);
	        }
        }
    }
}