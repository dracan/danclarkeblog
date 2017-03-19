using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Web.ViewModels
{
    public class PostViewModel : ViewModelBase
    {
        public BlogPost Post { get; set; }
        public string DisqusDomainName { get; set; }
    }
}