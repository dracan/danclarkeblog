using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.Search;

public class SearchViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("Search");
}