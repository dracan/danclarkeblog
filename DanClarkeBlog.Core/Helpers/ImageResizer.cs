using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DanClarkeBlog.Core.Helpers
{
    public class ImageResizer : IImageResizer
    {
        public byte[] Resize(byte[] source, int width)
        {
            using (var stream = new MemoryStream(source))
            {
                using (var image = Image.FromStream(stream))
                {
                    var resizedImaged = ResizeImage(image, width);

                    using (var destStream = new MemoryStream())
                    {
                        resizedImaged.Save(destStream, ImageFormat.Png);

                        return destStream.ToArray();
                    }
                }
            }
        }

        private static Image ResizeImage(Image image, int maxWidth)
        {
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            if (originalWidth <= maxWidth)
            {
                return image;
            }

            var percentWidth = maxWidth / (float)originalWidth;
            var newWidth = (int)(originalWidth * percentWidth);
            var newHeight = (int)(originalHeight * percentWidth);

            Image newImage = new Bitmap(newWidth, newHeight);

            using (var graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
    }
}