using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyCQRS.Azure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    ErrorDescription = table.Column<string>(nullable: true),
                    Executed = table.Column<bool>(nullable: false),
                    ExecutedAt = table.Column<DateTimeOffset>(nullable: true),
                    ExecutedBy = table.Column<Guid>(nullable: true),
                    Payload = table.Column<byte[]>(nullable: true),
                    ScheduledAt = table.Column<DateTimeOffset>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    SourceType = table.Column<string>(maxLength: 300, nullable: false),
                    AggregateId = table.Column<Guid>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    Payload = table.Column<byte[]>(maxLength: 2147483647, nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => new { x.SourceType, x.AggregateId, x.Version });
                    table.UniqueConstraint("AK_Events_AggregateId_SourceType_Version", x => new { x.AggregateId, x.SourceType, x.Version });
                });

            migrationBuilder.CreateTable(
                name: "Sagas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 500, nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    Payload = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sagas", x => new { x.Id, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    SourceType = table.Column<string>(maxLength: 300, nullable: false),
                    AggregateId = table.Column<Guid>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    Payload = table.Column<byte[]>(maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => new { x.SourceType, x.AggregateId, x.Version });
                    table.UniqueConstraint("AK_Snapshots_AggregateId_SourceType_Version", x => new { x.AggregateId, x.SourceType, x.Version });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Sagas");

            migrationBuilder.DropTable(
                name: "Snapshots");
        }
    }
}
