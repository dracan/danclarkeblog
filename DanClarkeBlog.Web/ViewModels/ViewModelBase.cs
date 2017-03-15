using System.Collections.Generic;

namespace DanClarkeBlog.Web.ViewModels
{
    public abstract class ViewModelBase
    {
        public List<DanClarkeBlog.Core.Models.BlogPost> FeaturedPosts { get; set; }
    }
}