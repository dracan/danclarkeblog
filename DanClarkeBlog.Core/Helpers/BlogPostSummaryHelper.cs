using System;
using System.Text.RegularExpressions;

namespace DanClarkeBlog.Core.Helpers
{
    public class BlogPostSummaryHelper
    {
        public string GetSummaryText(string content)
        {
            var indexOfSplitter = content.IndexOf("-----", StringComparison.Ordinal);

            if (indexOfSplitter == -1)
            {
                var match = Regex.Match(content, @"^\s*$", RegexOptions.Multiline);
                indexOfSplitter = match.Success ? match.Index : -1;
            }

            if (indexOfSplitter == -1)
            {
                indexOfSplitter = 200; // (dan) Make me a setting
            }

            return content.Substring(0, indexOfSplitter);
        }
    }
}
