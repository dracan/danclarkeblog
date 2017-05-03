using System;
using System.Text.RegularExpressions;

namespace DanClarkeBlog.Core.Helpers
{
    public class BlogPostSummaryHelper
    {
        private const int SummaryLength = 200; // (todo) Make me a setting

        internal static string ManualSplitterText => "<--->";

        public string GetSummaryText(string content)
        {
            var indexOfSplitter = content.IndexOf(ManualSplitterText, StringComparison.Ordinal);

            if (indexOfSplitter == -1)
            {
                var match = Regex.Match(content, @"^\s*$", RegexOptions.Multiline);
                indexOfSplitter = match.Success ? match.Index : -1;
            }

            if (content.Length < SummaryLength)
            {
                return content;
            }

            if (indexOfSplitter == -1)
            {
                indexOfSplitter = SummaryLength;
            }

            //(todo) Splitting by this Summary Length should really only break on whole words or sentences.
            //Not a big priority, as for most posts, I'll use the "-----" splitter to explicitly specify the break.

            return content.Substring(0, indexOfSplitter);
        }
    }
}
