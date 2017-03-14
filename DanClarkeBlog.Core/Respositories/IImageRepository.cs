using System.IO;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Respositories
{
    public interface IImageRepository
    {
        Task AddAsync(string fileReference, byte[] data);
    }
}
