using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DanClarkeBlog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configBuilder) =>
                {
                    var keyVaultUri = configBuilder.Build()["KeyVaultUri"];

                    if (!string.IsNullOrWhiteSpace(keyVaultUri))
                        configBuilder.AddAzureKeyVault(keyVaultUri, new DefaultKeyVaultSecretManager());
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging => logging.AddApplicationInsights());
    }
}
