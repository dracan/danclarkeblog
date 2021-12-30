using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.ProfilePicture;

public class ProfilePictureViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View("ProfilePicture");
}