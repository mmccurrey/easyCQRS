using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace EasyCQRS.Extensions.HealthCheck
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _path;
        private readonly TimeSpan _timeout;
        private readonly IEnumerable<IHealthCheck> _checks;

        public HealthCheckMiddleware(RequestDelegate next, IEnumerable<IHealthCheck> checks)
            : this(next, checks, "/health-check", TimeSpan.FromSeconds(30))
        { }

        public HealthCheckMiddleware(RequestDelegate next, IEnumerable<IHealthCheck> checks, string path, TimeSpan timeout)
        {
            _next = next;
            _checks = checks;
            _path = path;            
            _timeout = timeout;            
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == _path)
            {
                var timeoutTokenSource = new CancellationTokenSource(_timeout);
                var result = await RunChecksAsync(timeoutTokenSource.Token);
                var status = result.Status;

                if (status != HealthCheckStatus.Healthy)
                    context.Response.StatusCode = 503;
                
                await context.Response.WriteAsync(status.ToString());

                return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        public async Task<IHealthCheckResult> RunChecksAsync(CancellationToken cancellationToken)
        {
            var result = new CompositeHealthCheckResult();
            var tasks = _checks.Select(c => new {
                                                Check = c,
                                                Task = c.Check(cancellationToken)
                                            })
                                            .ToList();

            await Task.WhenAll(tasks.Select(c => c.Task));

            foreach (var task in tasks)
            {
                result.Add(task.Task.Result);
            }

            return result;
        }
    }
}
