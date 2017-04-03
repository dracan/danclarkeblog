namespace DanClarkeBlog.Core.Models
{
    public class TagCount
    {
        public string TagName { get; private set; }
        public int Count { get; private set; }

        public TagCount(string tagName, int count)
        {
            TagName = tagName;
            Count = count;
        }
    }
}