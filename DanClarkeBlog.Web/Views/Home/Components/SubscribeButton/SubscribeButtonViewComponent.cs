using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.SubscribeButton;

public class SubscribeButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("SubscribeButton");
}