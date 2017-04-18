using System;
using System.IO;
using System.Reflection;

namespace DanClarkeBlog.Core.Tests
{
    public class TestHelper
    {
        public static string GetAbsolutePath(string relativePath)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var basePath = Path.GetDirectoryName(codeBasePath);
            return basePath == null ? relativePath : Path.Combine(basePath, relativePath);
        }
    }
}