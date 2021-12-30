using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.PostListing;

public class PostListingViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(HomeViewModel model) =>
        View("PostListing", model);
}