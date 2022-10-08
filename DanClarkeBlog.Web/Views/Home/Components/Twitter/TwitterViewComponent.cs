using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.Twitter;

public class TwitterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("Twitter");
}
