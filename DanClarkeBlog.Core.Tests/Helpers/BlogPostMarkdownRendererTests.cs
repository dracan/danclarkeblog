using DanClarkeBlog.Core.Helpers;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class BlogPostMarkdownRendererTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData("", "/SomePostDir", "")]
        [InlineData("abc", "abc", "abc")]
        [InlineData(@"![](images/Banner.jpg)", "/SomePostDir", "![](https://danclarkeblog.blob.core.windows.net/images/somepostdir/banner.jpg)")]
        [InlineData(@"![](images/Banner.jpg)![](images/Banner.jpg)", "/SomePostDir", "![](https://danclarkeblog.blob.core.windows.net/images/somepostdir/banner.jpg)![](https://danclarkeblog.blob.core.windows.net/images/somepostdir/banner.jpg)")]
        [InlineData(@"![](images/Banner.jpg)", "/Shared/SomePostDir", "![](https://danclarkeblog.blob.core.windows.net/images/somepostdir/banner.jpg)")]
        public void UpdateImagePathsTests(string source, string imageFolder, string expectedResult)
        {
            var result = BlogPostMarkdownRenderer.UpdateImagePaths(source, imageFolder);
            Assert.Equal(expectedResult, result);
        }
    }
}