using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.Navbar;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("Navbar");
}