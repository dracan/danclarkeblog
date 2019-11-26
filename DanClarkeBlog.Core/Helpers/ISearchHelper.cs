using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Helpers
{
    public interface ISearchHelper
    {
        Task<BlogPostListing> SearchAsync(string searchTerm, int offset, int count);
    }
}