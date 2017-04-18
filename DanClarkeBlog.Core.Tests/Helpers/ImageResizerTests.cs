using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DanClarkeBlog.Core.Helpers;
using Xunit;

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

            Image sourceImage = new Bitmap(sourceWidth, sourceHeight, PixelFormat.Format24bppRgb);

            using (var sourceImageStream = new MemoryStream())
            {
                sourceImage.Save(sourceImageStream, ImageFormat.Png);

                var sourceImageData = sourceImageStream.ToArray();

                var destImageData = sut.Resize(sourceImageData, maxWidth);

                Assert.NotNull(destImageData);

                using (var ms = new MemoryStream(destImageData))
                {
                    var destImage = Image.FromStream(ms);

                    Assert.NotNull(destImage);
                    Assert.Equal(expectedWidth, destImage.Width);
                    Assert.Equal(expectedHeight, destImage.Height);
                }
            }
        }
    }
}
