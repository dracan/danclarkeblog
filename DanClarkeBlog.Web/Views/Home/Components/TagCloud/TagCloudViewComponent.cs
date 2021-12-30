using System.Collections.Generic;
using DanClarkeBlog.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.TagCloud;

public class TagCloudViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(List<TagCount> tags) =>
        View("TagCloud", tags);
}