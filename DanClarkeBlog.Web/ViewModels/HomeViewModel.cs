using System.Collections.Generic;

namespace DanClarkeBlog.Web.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public List<DanClarkeBlog.Core.Models.BlogPost> Posts { get; set; }
    }
}