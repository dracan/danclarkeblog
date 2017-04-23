using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using Settings = DanClarkeBlog.Core.Settings;
using NLog.Extensions.Logging;

namespace DanClarkeBlog.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            //env.ConfigureNLog("nlog.config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(config =>
                            {
                                config.RespectBrowserAcceptHeader = true;
                            })
                    .AddXmlSerializerFormatters();

            services.AddOptions();
            services.Configure<Settings>(Configuration);

            var sp = services.BuildServiceProvider();
            var settings = sp.GetService<IOptions<Settings>>();

            var container = WebBootstrapper.Init(services, settings.Value, new NLogLoggerImpl(LogManager.GetLogger("")));

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "tags",
                    template: "tags/{tag}",
                    defaults: new { Controller = "Home", Action = "Index"});

                routes.MapRoute(
                    name: "rss",
                    template: "rss",
                    defaults: new { Controller = "Home", Action = "RssFeed"});

                routes.MapRoute(
                    name: "atom",
                    template: "atom",
                    defaults: new { Controller = "Home", Action = "AtomFeed"});

                routes.MapRoute(
                    name: "blogPost",
                    template: "{route}",
                    defaults: new { controller = "Home", action = "BlogPost" });
            });
        }
    }
}
