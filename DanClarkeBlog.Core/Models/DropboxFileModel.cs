namespace DanClarkeBlog.Core.Models
{
    public class DropboxFileModel
    {
        public string Name { get; }
        public string PathLower { get; }

        public DropboxFileModel(string name, string pathLower)
        {
            Name = name;
            PathLower = pathLower;
        }
    }
}