namespace DanClarkeBlog.Core.Helpers
{
    public interface IImageResizer
    {
        byte[] Resize(byte[] source, int width);
    }
}