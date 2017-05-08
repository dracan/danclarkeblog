using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Models
{
    public class BlogImageData
    {
        public string FileName { get; set; }
        public string PostFolder { get; set; }
        public Task<byte[]> ImageDataTask { get; set; }
    }
}