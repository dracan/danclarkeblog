using System.Text.RegularExpressions;

namespace DanClarkeBlog.Core.Helpers
{
    public class BlogPostMarkdownRenderer : IBlogPostRenderer
    {
        public string Render(string source)
        {
            return ConvertMarkdownToHtml(source);
        }

        private static string ConvertMarkdownToHtml(string markdown)
        {
            // The regular expression is to convert the relative path on the filesystem to path that will work on the web.
            // This allows editing the local Markdown files and being able to see the images in your markdown editor.
            return Markdig.Markdown.ToHtml(Regex.Replace(markdown, "../.*?/(images/.*)", "/$1"));
        }
    }
}