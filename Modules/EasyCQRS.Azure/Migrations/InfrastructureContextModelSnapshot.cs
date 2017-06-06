using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using EasyCQRS.Azure;

namespace EasyCQRS.Azure.Migrations
{
    [DbContext(typeof(InfrastructureContext))]
    partial class InfrastructureContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EasyCQRS.Azure.CommandEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CorrelationId");

                    b.Property<string>("ErrorDescription");

                    b.Property<bool>("Executed");

                    b.Property<DateTimeOffset?>("ExecutedAt");

                    b.Property<Guid?>("ExecutedBy");

                    b.Property<string>("FullName");

                    b.Property<byte[]>("Payload");

                    b.Property<DateTimeOffset>("ScheduledAt");

                    b.Property<bool>("Success");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("EasyCQRS.Azure.EventEntity", b =>
                {
                    b.Property<string>("SourceType")
                        .HasMaxLength(300);

                    b.Property<Guid>("AggregateId");

                    b.Property<long>("Version");

                    b.Property<Guid>("CorrelationId");

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("FullName");

                    b.Property<byte[]>("Payload")
                        .HasMaxLength(2147483647);

                    b.Property<string>("Type");

                    b.HasKey("SourceType", "AggregateId", "Version");

                    b.HasAlternateKey("AggregateId", "SourceType", "Version");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EasyCQRS.Azure.SagaEntity", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("FullName")
                        .HasMaxLength(500);

                    b.Property<bool>("Completed");

                    b.Property<Guid>("CorrelationId");

                    b.Property<byte[]>("Payload");

                    b.Property<string>("Type");

                    b.HasKey("Id", "FullName");

                    b.HasAlternateKey("FullName", "Id");

                    b.ToTable("Sagas");
                });

            modelBuilder.Entity("EasyCQRS.Azure.SnapshotEntity", b =>
                {
                    b.Property<string>("SourceType")
                        .HasMaxLength(300);

                    b.Property<Guid>("AggregateId");

                    b.Property<long>("Version");

                    b.Property<byte[]>("Payload")
                        .HasMaxLength(2147483647);

                    b.HasKey("SourceType", "AggregateId", "Version");

                    b.HasAlternateKey("AggregateId", "SourceType", "Version");

                    b.ToTable("Snapshots");
                });
        }
    }
}
