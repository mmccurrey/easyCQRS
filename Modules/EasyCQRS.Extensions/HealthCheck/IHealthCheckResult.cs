using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Extensions.HealthCheck
{
    public interface IHealthCheckResult
    {
        HealthCheckStatus Status { get; }
        string Description { get; }

        IReadOnlyDictionary<string, string> Properties { get; }
    }
}
