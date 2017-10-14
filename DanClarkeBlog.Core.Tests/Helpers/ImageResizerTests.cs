using System.IO;
using DanClarkeBlog.Core.Helpers;
using SixLabors.ImageSharp;
using Xunit;
using Img = SixLabors.ImageSharp.Image;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class ImageResizerTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(100, 100, 50, 50, 50)]
        [InlineData(50, 150, 50, 50, 150)]
        [InlineData(10, 150, 50, 10, 150)]
        public void ImageResizerTest(int sourceWidth, int sourceHeight, int maxWidth, int expectedWidth, int expectedHeight)
        {
            var sut = new ImageResizer();

            using (var image = new Image<Rgba32>(sourceWidth, sourceHeight))
            {
                using (var sourceImageStream = new MemoryStream())
                {
                    image.SaveAsPng(sourceImageStream);

                    var sourceImageBytes = sourceImageStream.ToArray();

                    var destImageData = sut.Resize(sourceImageBytes, maxWidth);

                    Assert.NotNull(destImageData);

                    using (var destImage = Img.Load<Rgba32>(destImageData))
                    {
                        Assert.NotNull(destImage);
                        Assert.Equal(expectedWidth, destImage.Width);
                        Assert.Equal(expectedHeight, destImage.Height);
                    }
                }
            }
        }
    }
}
