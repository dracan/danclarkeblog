using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.Mastodon;

public class MastodonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("Mastodon");
}
