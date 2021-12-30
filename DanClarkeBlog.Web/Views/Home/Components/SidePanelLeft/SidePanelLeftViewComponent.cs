using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.SidePanelLeft;

public class SidePanelLeftViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ViewModelBase model) =>
        View("SidePanelLeft", model);
}