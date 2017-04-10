namespace DanClarkeBlog.Core.Models
{
    public class DropboxFileModel
    {
        public string Name { get; private set; }
        public string PathLower { get; private set; }

        public DropboxFileModel(string name, string pathLower)
        {
            Name = name;
            PathLower = pathLower;
        }
    }
}