using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Azure
{
    internal class InfrastructureDbContextFactory : IDbContextFactory<InfrastructureContext>
    {
        public InfrastructureContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<InfrastructureContext>();            

            builder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=Infrastructure;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new InfrastructureContext(builder.Options);
        }
    }
}
