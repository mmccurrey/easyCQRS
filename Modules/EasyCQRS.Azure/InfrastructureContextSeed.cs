using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{
    public class InfrastructureContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            var context = (InfrastructureContext)applicationBuilder
                .ApplicationServices.GetService(typeof(InfrastructureContext));

            await context.Database.MigrateAsync();
        }
    }
}
