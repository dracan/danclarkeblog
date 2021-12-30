using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.DarkModeToggle;

public class DarkModeToggleViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("DarkModeToggle");
}