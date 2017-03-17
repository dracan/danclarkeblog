using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Data;
using DanClarkeBlog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DanClarkeBlog.Core.Respositories //(dan) Spelt wrong!
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
                return await ctx.BlogPosts
                    .OrderByDescending(x => x.PublishDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetWithConditionAsync(Func<BlogPost, bool> conditionFunc, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_settings))
            {
                return await ctx.BlogPosts
                    .OrderByDescending(x => x.PublishDate)
                    .Where(conditionFunc).AsQueryable().ToListAsync(cancellationToken);
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

        public async Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_settings))
            {
                var existing = await ctx.BlogPosts.FirstOrDefaultAsync(x => x.Title == post.Title, cancellationToken);

                if (existing == null)
                {
                    await ctx.BlogPosts.AddAsync(post, cancellationToken);
                }
                else
                {
                    existing.UpdateFrom(post);
                }

                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_settings))
            {
                ctx.BlogPosts.RemoveRange(postsToDelete);
                await ctx.SaveChangesAsync(cancellationToken);
            }
        }
    }
}