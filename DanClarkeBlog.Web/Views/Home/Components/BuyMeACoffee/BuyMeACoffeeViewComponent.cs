using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.BuyMeACoffee;

public class BuyMeACoffeeViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("BuyMeACoffee");
}