using System;

namespace DanClarkeBlog.Core.Models
{
    public class DropboxCursor
    {
        public Guid Id { get; set; }
        public string Cursor { get; set; }
    }
}