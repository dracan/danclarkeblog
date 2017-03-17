using System.Collections.Generic;

namespace DanClarkeBlog.Web.ViewModels
{
    public abstract class ViewModelBase
    {
        public List<Core.Models.BlogPost> FeaturedPosts { get; set; }
        public List<Core.Models.BlogPost> RecentPosts { get; set; }
    }
}