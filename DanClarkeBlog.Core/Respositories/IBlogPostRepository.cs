using System.Collections.Generic;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Respositories
{
    public interface IBlogPostRepository
    {
        IEnumerable<BlogPost> GetAll();
    }
}