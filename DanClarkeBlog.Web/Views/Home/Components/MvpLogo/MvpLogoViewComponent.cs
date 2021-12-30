using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.MvpLogo;

public class MvpLogoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("MvpLogo");
}