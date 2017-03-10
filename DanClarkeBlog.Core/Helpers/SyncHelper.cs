using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;

namespace DanClarkeBlog.Core.Helpers
{
    public class SyncHelper
    {
        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo, IBlogPostRepository destRepo, CancellationToken cancellationToken)
        {
            var sourcePosts = await sourceRepo.GetAllAsync(cancellationToken);
            // var destPosts = await destRepo.GetAllAsync();

            foreach(var sourcePost in sourcePosts)
            {
                await destRepo.AddOrUpdateAsync(sourcePost, cancellationToken);
            }

            //(todo) Support deletion
        }
    }
}