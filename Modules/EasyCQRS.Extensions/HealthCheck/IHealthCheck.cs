using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCQRS.Extensions.HealthCheck
{
    public interface IHealthCheck
    {
        Task<IHealthCheckResult> Check(CancellationToken cancellationToken);
    }
}
