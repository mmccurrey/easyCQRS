using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyCQRS.Azure.Migrations
{
    public partial class InfrastructureV015 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExecutedBy",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IntegrationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    ExecutedBy = table.Column<Guid>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    Payload = table.Column<byte[]>(maxLength: 2147483647, nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEvents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEvents");

            migrationBuilder.DropColumn(
                name: "ExecutedBy",
                table: "Events");
        }
    }
}
