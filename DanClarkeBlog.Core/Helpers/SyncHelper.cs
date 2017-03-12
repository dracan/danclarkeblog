using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;

namespace DanClarkeBlog.Core.Helpers
{
    public class SyncHelper
    {
        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo, IBlogPostRepository destRepo, CancellationToken cancellationToken)
        {
            var sourcePosts = (await sourceRepo.GetAllAsync(cancellationToken)).ToList();
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