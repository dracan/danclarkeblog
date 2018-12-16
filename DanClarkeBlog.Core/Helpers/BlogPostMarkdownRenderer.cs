using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Markdig;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class BlogPostMarkdownRenderer : IBlogPostRenderer
    {
        private readonly Settings _settings;
        private readonly MarkdownPipeline _pipeline;

        public BlogPostMarkdownRenderer(Settings settings)
        {
            _settings = settings;
            _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        public string Render(string source, string postFolderName)
        {
            return ConvertMarkdownToHtml(source, postFolderName);
        }

        private string ConvertMarkdownToHtml(string markdown, string postFolderName)
        {
            return Markdown.ToHtml(UpdateImagePaths(markdown, postFolderName), _pipeline);
        }

        internal string UpdateImagePaths(string source, string postFolderName)
        {
            // We only want the last part of the path, as we might have temprarily put the blog post
            // in a sub folder (eg. a shared "WIP" Dropbox folder).
            var leafPostFolderName = new DirectoryInfo(postFolderName).Name;

            // Remove the manual splitter text if it exists.
            source = source.Replace(BlogPostSummaryHelper.ManualSplitterText, "");

            // The regular expression is to convert the relative path on the filesystem to path that will work on the web.
            // This allows editing the local Markdown files and being able to see the images in your markdown editor.
            return Regex.Replace(source, @"(!\[.*?\]\()images/(.*?\))", x => $"{x.Groups[1].Value}{_settings.BaseImageUri}/{leafPostFolderName}/{x.Groups[2].Value}".ToLower());
        }
    }
}