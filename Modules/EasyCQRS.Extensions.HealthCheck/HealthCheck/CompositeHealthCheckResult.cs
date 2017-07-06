using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyCQRS.Extensions.HealthCheck
{
    public class CompositeHealthCheckResult : IHealthCheckResult
    {
        private readonly Dictionary<string, IHealthCheckResult> _results;

        public CompositeHealthCheckResult()
        {
            _results = new Dictionary<string, IHealthCheckResult>();
        }

        public HealthCheckStatus Status
        {
            get
            {
                var checkStatuses = new HashSet<HealthCheckStatus>(_results.Values.Select(x => x.Status));
               
                if (checkStatuses.Count == 1)
                {
                    return checkStatuses.First();
                }

                if (checkStatuses.Contains(HealthCheckStatus.Healthy))
                {
                    return HealthCheckStatus.PartialHealthy;
                }

                return HealthCheckStatus.Unhealthy;
            }
        }

        public string Description => string.Join(Environment.NewLine, _results.Select(r => $"{r.Key}: {r.Value.Description}"));

        public IReadOnlyDictionary<string, string> Properties
        {
            get
            {
                var properties = new Dictionary<string, string>();

                if (_results != null)
                {
                    foreach (var result in _results)
                    {
                        if (result.Value.Properties != null)
                        {
                            foreach (var property in result.Value.Properties)
                            {
                                properties.Add(result.Key + "." + property.Key, property.Value);
                            }
                        }
                    }
                }

                return properties;
            }
        }

        public void Add(IHealthCheckResult checkResult)
        {
            _results[checkResult.GetType().Name] = checkResult ?? throw new ArgumentNullException(nameof(checkResult));
        }
    }
}
