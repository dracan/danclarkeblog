using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.Paging;

public class PagingViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(HomeViewModel model) =>
        View("Paging", model);
}