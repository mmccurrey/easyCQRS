using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace KJDevSec.Azure
{
    class InfrastructureContext: DbContext
    {
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<CommandEntity> Commands { get; set; }
        public DbSet<SagaEntity> Sagas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            {
                if (optionsBuilder.IsConfigured == false)
                {
                    optionsBuilder.UseSqlServer(
                   @"Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreFullNet;
                       Integrated Security=True;");
                }

                base.OnConfiguring(optionsBuilder);
            }
        }
    }
}
