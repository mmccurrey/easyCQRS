using EasyCQRS.Extensions.HealthCheck;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS
{
    public static class MiddlewareExtensions
    {
        public static void UseHealthCheck(this IApplicationBuilder app)
        {
            app.UseMiddleware<HealthCheckMiddleware>();
        }

        public static void UseHealthCheck(this IApplicationBuilder app, string path, TimeSpan timeout)
        {
            app.UseMiddleware<HealthCheckMiddleware>(path, timeout);
        }
    }
}
