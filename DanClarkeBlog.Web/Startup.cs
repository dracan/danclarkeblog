using DanClarkeBlog.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(config =>
                 {
                     config.RespectBrowserAcceptHeader = true;
                 }).AddXmlSerializerFormatters();

            services.AddLogging();
            services.AddApplicationInsightsTelemetry();

            services.AddOptions();
            services.Configure<Settings>(Configuration.GetSection("Blog"));

            var sp = services.BuildServiceProvider();
            var settings = sp.GetService<IOptions<Settings>>();

            WebBootstrapper.Init(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "tags",
                    pattern: "tags/{tag}",
                    defaults: new { Controller = "Home", Action = "Index"});

                endpoints.MapControllerRoute(
                    name: "search",
                    pattern: "search",
                    defaults: new { Controller = "Home", Action = "Search"});

                endpoints.MapControllerRoute(
                    name: "rss",
                    pattern: "rss",
                    defaults: new { Controller = "Home", Action = "RssFeed"});

                endpoints.MapControllerRoute(
                    name: "atom",
                    pattern: "atom",
                    defaults: new { Controller = "Home", Action = "AtomFeed"});

                endpoints.MapControllerRoute(
                    name: "blogPost",
                    pattern: "{route}",
                    defaults: new { controller = "Home", action = "BlogPost" });
            });
        }
    }
}
