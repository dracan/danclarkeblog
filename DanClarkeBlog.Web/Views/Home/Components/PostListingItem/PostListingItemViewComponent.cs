using DanClarkeBlog.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.PostListingItem;

public class PostListingItemViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(BlogPost item) =>
        View("PostListingItem", item);
}