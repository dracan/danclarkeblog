using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Views.Home.Components.SidePanelRight;

public class SidePanelRightViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ViewModelBase model) =>
        View("SidePanelRight", model);
}