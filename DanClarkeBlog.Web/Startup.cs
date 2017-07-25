using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using Settings = DanClarkeBlog.Core.Settings;

namespace DanClarkeBlog.Web
{
    public class Startup
    {
        private readonly bool _isDevelopment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _isDevelopment = env.IsDevelopment();

            if (env.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.LiterateConsole()
                    .CreateLogger();
            }

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
                                config.Filters.Add(new RequireHttpsAttribute());
                            })
                    .AddXmlSerializerFormatters();

            services.AddOptions();
            services.Configure<Settings>(Configuration.GetSection("Blog"));

            var sp = services.BuildServiceProvider();
            var settings = sp.GetService<IOptions<Settings>>();

            services.AddApplicationInsightsTelemetry(options =>
                {
                    options.DeveloperMode = settings.Value.ApplicationInsightsDeveloperMode;
                    options.InstrumentationKey = settings.Value.ApplicationInsightsInstrumentationKey;
                });

            if (!_isDevelopment)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.ApplicationInsightsTraces(settings.Value.ApplicationInsightsInstrumentationKey)
                    .CreateLogger();
            }

            var container = WebBootstrapper.Init(services, settings.Value);

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

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

            // ReSharper disable once UnusedVariable
            var options = new RewriteOptions()
                .AddRedirectToHttps(301);

            app.UseRewriter(options);

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
