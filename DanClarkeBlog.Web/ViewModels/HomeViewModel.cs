using System.Collections.Generic;

namespace DanClarkeBlog.Web.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public List<Core.Models.BlogPost> Posts { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}