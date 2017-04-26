using System.IO;
using System.Text.RegularExpressions;

namespace DanClarkeBlog.Core.Helpers
{
    public class BlogPostMarkdownRenderer : IBlogPostRenderer
    {
        public string Render(string source, string postFolderName)
        {
            return ConvertMarkdownToHtml(source, postFolderName);
        }

        private static string ConvertMarkdownToHtml(string markdown, string postFolderName)
        {
            return Markdig.Markdown.ToHtml(UpdateImagePaths(markdown, postFolderName));
        }

        internal static string UpdateImagePaths(string source, string postFolderName)
        {
            // We only want the last part of the path, as we might have temprarily put the blog post
            // in a sub folder (eg. a shared "WIP" Dropbox folder).
            var leafPostFolderName = new DirectoryInfo(postFolderName).Name;

            // The regular expression is to convert the relative path on the filesystem to path that will work on the web.
            // This allows editing the local Markdown files and being able to see the images in your markdown editor.
            return Regex.Replace(source, @"(!\[.*?\]\()images/(.*?\))", x => $"{x.Groups[1].Value}https://danclarkeblog.blob.core.windows.net/images/{leafPostFolderName}/{x.Groups[2].Value}".ToLower()); //(todo) Don't hardcode this
        }
    }
}