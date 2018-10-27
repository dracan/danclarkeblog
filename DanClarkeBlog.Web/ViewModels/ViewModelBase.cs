using System.Collections.Generic;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Web.ViewModels
{
    public abstract class ViewModelBase
    {
        public List<BlogPost> FeaturedPosts { get; set; }
        public List<BlogPost> RecentPosts { get; set; }
        public List<TagCount> Tags { get; set; }
        public string ProfilePicUri { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
        public string VersionNumber { get; set; }
    }
}