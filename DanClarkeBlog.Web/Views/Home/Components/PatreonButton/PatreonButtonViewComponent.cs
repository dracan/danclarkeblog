using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.PatreonButton;

public class PatreonButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("PatreonButton");
}