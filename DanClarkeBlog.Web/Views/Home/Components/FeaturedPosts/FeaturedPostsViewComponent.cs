using System.Collections.Generic;
using DanClarkeBlog.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.FeaturedPosts;

public class FeaturedPostsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(List<BlogPost> posts) =>
        View("FeaturedPosts", posts);
}