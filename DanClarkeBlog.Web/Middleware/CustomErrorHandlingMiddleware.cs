using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace DanClarkeBlog.Web.Middleware
{
    public class CustomErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly Counter MetricsCounter = Metrics.CreateCounter("Blog_Exception_Counter",
            "Number of unhandled exceptions resulting in 500 errors");

        public CustomErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                MetricsCounter.Inc();
                throw;
            }
        }
    }
  
    public static class CustomErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomErrorHandlingMiddleware>();
        }
    } 
}