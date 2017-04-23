using System.Threading;
using System.Threading.Tasks;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IFeedGenerator
    {
        Task<string> GenerateRssAsync(CancellationToken cancelationToken);
        Task<string> GenerateAtomAsync(CancellationToken cancelationToken);
    }
}