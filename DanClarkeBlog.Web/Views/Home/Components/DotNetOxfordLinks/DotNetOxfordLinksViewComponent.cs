using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.DotNetOxfordLinks;

public class DotNetOxfordLinksViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("DotNetOxfordLinks");
}