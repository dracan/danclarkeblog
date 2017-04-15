using DanClarkeBlog.Core.Helpers;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class BlogPostSummaryHelperTests
    {
        //(todo) Add more tests for this class

        [Fact, Trait("Category", "Unit")]
        public void GetSummaryText_WhenContentIsLessThanSummarySize()
        {
            var sut = new BlogPostSummaryHelper();
            var result = sut.GetSummaryText("abc");
            Assert.Equal("abc", result);
        }
    }
}
