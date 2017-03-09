using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Data;
using DanClarkeBlog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DanClarkeBlog.Core.Respositories
{
    public class BlogPostAzureSqlRepository : IBlogPostRepository
    {
        private readonly Settings _settings;

        public BlogPostAzureSqlRepository(Settings settings)
        {
            _settings = settings;
        }

        public void CreateDatabase()
        {
            using (var ctx = new DataContext(_settings))
            {
                ctx.Database.EnsureCreated();
            }
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_settings))
            {
                return await ctx.BlogPosts.ToListAsync(cancellationToken);
            }
        }

        public async Task AddAsync(BlogPost post, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_settings))
            {
                await ctx.BlogPosts.AddAsync(post, cancellationToken);
                await ctx.SaveChangesAsync(cancellationToken);
            }
        }
    }
}