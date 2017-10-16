using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DanClarkeBlog.Functions
{
    public static class AssemblyBindingRedirectHelper
    {
        ///<summary>
        /// Reads the "BindingRedirecs" field from the app settings and applies the redirection on the
        /// specified assemblies
        /// </summary>

        public static void ConfigureBindingRedirects()
        {
            var redirects = GetBindingRedirects();
            redirects.ForEach(RedirectAssembly);
        }

        private static List<BindingRedirect> GetBindingRedirects()
        {
            return new List<BindingRedirect>()
            {
                new BindingRedirect
                {
                    ShortName = "System.ValueTuple",
                    PublicKeyToken = "cc7b13ffcd2ddd51",
                    RedirectToVersion = "4.0.2.0",
                }
            };
        }

        private static void RedirectAssembly(BindingRedirect bindingRedirect)
        {
            ResolveEventHandler handler = null;
            handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != bindingRedirect.ShortName)
                {
                    return null;
                }
                var targetPublicKeyToken = new AssemblyName("x, PublicKeyToken=" + bindingRedirect.PublicKeyToken).GetPublicKeyToken();
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);
                requestedAssembly.Version = new Version(bindingRedirect.RedirectToVersion);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;
                AppDomain.CurrentDomain.AssemblyResolve -= handler;
                return Assembly.Load(requestedAssembly);
            };
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }

        public class BindingRedirect
        {
            public string ShortName { get; set; }
            public string PublicKeyToken { get; set; }
            public string RedirectToVersion { get; set; }
        }
    }
}