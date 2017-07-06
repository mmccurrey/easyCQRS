using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCQRS.Extensions.HealthCheck
{
    public class PingHealthCheck : IHealthCheck
    {
        private readonly string _host;
        private readonly TimeSpan _timeout;

        public PingHealthCheck(string host, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentException(nameof(host));

            _host = host;
            _timeout = timeout;
        }

        public async Task<IHealthCheckResult> Check(CancellationToken cancellationToken)
        {
            var ping = new Ping();
            var result = await ping.SendPingAsync(_host, (int)_timeout.TotalMilliseconds);

            var checkResult = (result.Status == IPStatus.Success) ? HealthCheckStatus.Healthy : HealthCheckStatus.Unhealthy;

            return new HealthCheckResult(
                checkResult, 
                result.Status.ToString(),
                new Dictionary<string, string>
                {
                    ["host"] = _host,
                    ["timeout"] = _timeout.TotalMilliseconds.ToString(),
                    ["roundstrip"] = result.RoundtripTime.ToString()
                });
        }
    }
}
