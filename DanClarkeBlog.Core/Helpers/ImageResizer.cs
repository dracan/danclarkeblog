using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace DanClarkeBlog.Core.Helpers
{
    public class ImageResizer : IImageResizer
    {
        public byte[] Resize(byte[] source, int width)
        {
            using (var image = Image.Load(source))
            {
                var originalWidth = image.Width;
                var originalHeight = image.Height;

                if (originalWidth <= width)
                {
                    return source;
                }

                var percentWidth = width / (float) originalWidth;
                var newWidth = (int) (originalWidth * percentWidth);
                var newHeight = (int) (originalHeight * percentWidth);

                image.Mutate(x => x.Resize(newWidth, newHeight));

                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, Image.DetectFormat(source));

                    return outputStream.ToArray();
                }
            }
        }
    }
}