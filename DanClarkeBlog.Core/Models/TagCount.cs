namespace DanClarkeBlog.Core.Models
{
    public class TagCount
    {
        public string TagName { get; }
        public int Count { get; }

        public TagCount(string tagName, int count)
        {
            TagName = tagName;
            Count = count;
        }
    }
}