using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using Settings = DanClarkeBlog.Core.Settings;
using NLog.Extensions.Logging;
using NLog.Web;
using ILogger = DanClarkeBlog.Core.Helpers.ILogger;

namespace DanClarkeBlog.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");

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
            services.AddMvc();

            services.AddOptions();
            services.Configure<Settings>(Configuration);

            var sp = services.BuildServiceProvider();
            var settings = sp.GetService<IOptions<Settings>>();

            // Setup Autofac
            var builder = new ContainerBuilder();
            builder.Register(_ => settings.Value);

            //if (string.IsNullOrWhiteSpace(settings.Value.BlogFileSystemRootPath))
                builder.RegisterType<BlogPostSqlServerRepository>().As<IBlogPostRepository>();
            //else
            //    builder.RegisterType<BlogPostFileSystemRepository>().As<IBlogPostRepository>();

            builder.RegisterType<BlogPostSummaryHelper>();
            builder.RegisterType<BlogPostMarkdownRenderer>().As<IBlogPostRenderer>();
            builder.RegisterType<AzureImageRepository>().As<IImageRepository>();
            builder.Register<ILogger>(x => new NLogLoggerImpl(LogManager.GetLogger("")));
            builder.Populate(services);

            return new AutofacServiceProvider(builder.Build());
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
                    name: "blogPost",
                    template: "{route}",
                    defaults: new { controller = "Home", action = "BlogPost" });
            });
        }
    }
}
