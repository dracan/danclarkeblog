namespace DanClarkeBlog.Core.Helpers
{
    public class ImageResizer : IImageResizer
    {
        public byte[] Resize(byte[] source, int width)
        {
            // (todo) Implement this. For now, I'm going to manually resize images on dropbox whilst writing the posts.
            // I started this, then decided to postpone this task until later, as there's higher priority tasks. Didn't
            // want to undo this interface and this class I've already created though.
            return source;
        }
    }
}