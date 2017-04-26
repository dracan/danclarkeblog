namespace DanClarkeBlog.Core.Helpers
{
    public interface IBlogPostRenderer
    {
        string Render(string source, string postFolderName);
    }
}