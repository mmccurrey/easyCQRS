using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace KJDevSec.Azure.EventSourcing
{
    class SQLEventSourcingContext: DbContext
    {
        public DbSet<SQLEvent> Events { get; set; }

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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<SQLEvent>()
                .ToTable("Events");
        }
    }
}
