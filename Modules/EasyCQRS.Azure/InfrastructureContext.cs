using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using JetBrains.Annotations;

namespace EasyCQRS.Azure
{
    internal class InfrastructureContext: DbContext
    {
        public InfrastructureContext(DbContextOptions<InfrastructureContext> options)
            : base(options)
        {
        }

        public DbSet<EventEntity> Events { get; set; }
        public DbSet<CommandEntity> Commands { get; set; }
        public DbSet<SagaEntity> Sagas { get; set; }
        public DbSet<SnapshotEntity> Snapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventEntity>()
                        .HasKey(e => new { e.SourceType, e.AggregateId, e.Version });

            modelBuilder.Entity<SagaEntity>()
                        .HasKey(s => new { s.Id, s.FullName });

            modelBuilder.Entity<SnapshotEntity>()
                        .HasKey(s => new { s.SourceType, s.AggregateId, s.Version });
        }
    }
}
