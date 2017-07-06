using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Extensions.HealthCheck
{
    public class HealthCheckResult: IHealthCheckResult
    {
        public HealthCheckResult(HealthCheckStatus status)
               : this(status, string.Empty, null)
        {
        }

        public HealthCheckResult(HealthCheckStatus status, string description)
            : this(status, description, null)
        {
        }

        public HealthCheckResult(HealthCheckStatus status, string description, IReadOnlyDictionary<string, string> properties)
        {
            this.Status = status;
            this.Description = description;
            this.Properties = properties;
        }

        public HealthCheckStatus Status { get; }

        public string Description { get; }

        public IReadOnlyDictionary<string, string> Properties { get; }
    }
}
