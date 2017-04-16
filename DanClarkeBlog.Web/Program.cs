using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DanClarkeBlog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSetting("detailedErrors", "true") //(todo) I've added this to debug a 500 issue, it probably shouldn't be in prod!
                .UseIISIntegration()
                .UseStartup<Startup>()
                .CaptureStartupErrors(true) //(todo) I've added this to debug a 500 issue, it probably shouldn't be in prod!
                .Build();

            host.Run();
        }
    }
}
