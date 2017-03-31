using System;

namespace DanClarkeBlog.Core.Dropbox
{
    public class DropboxApiResponseGetFileContent
    {
        public string name { get; set; }
        public string path_lower { get; set; }
        public string path_display { get; set; }
        public string id { get; set; }
        public DateTime client_modified { get; set; }
        public DateTime server_modified { get; set; }
        public string rev { get; set; }
        public int size { get; set; }
        public string content_hash { get; set; }
    }
}