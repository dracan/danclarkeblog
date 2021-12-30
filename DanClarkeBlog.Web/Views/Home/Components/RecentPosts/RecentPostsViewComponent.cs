using System.Collections.Generic;
using DanClarkeBlog.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.RecentPosts;

public class RecentPostsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(List<BlogPost> posts) =>
        View("RecentPosts", posts);
}