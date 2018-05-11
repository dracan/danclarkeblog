using System.Threading.Tasks;

namespace DanClarkeBlog.Tasks
{
    public interface ITask
    {
        Task ExecuteAsync(string message);
    }
}