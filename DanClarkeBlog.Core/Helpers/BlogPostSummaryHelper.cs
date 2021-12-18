using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Core.Helpers
{
    public class BlogPostSummaryHelper
    {
        private readonly Settings _settings;

        public BlogPostSummaryHelper(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        internal static string ManualSplitterText => "<--->";

        public string GetSummaryText(string content)
        {
            var indexOfSplitter = content.IndexOf(ManualSplitterText, StringComparison.Ordinal);

            if (indexOfSplitter == -1)
            {
                var match = Regex.Match(content, @"^\s*$", RegexOptions.Multiline);
                indexOfSplitter = match.Success ? match.Index : -1;
            }

            if (content.Length < _settings.PostPreviewLength)
            {
                return content;
            }

            if (indexOfSplitter == -1)
            {
                indexOfSplitter = _settings.PostPreviewLength;
            }

            //(todo) Splitting by this Summary Length should really only break on whole words or sentences.
            //Not a big priority, as for most posts, I'll use the "-----" splitter to explicitly specify the break.

            return content.Substring(0, indexOfSplitter);
        }
    }
}
