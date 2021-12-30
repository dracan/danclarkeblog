using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.Footer;

public class FooterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string versionNumber) =>
        View("Footer", versionNumber);
}