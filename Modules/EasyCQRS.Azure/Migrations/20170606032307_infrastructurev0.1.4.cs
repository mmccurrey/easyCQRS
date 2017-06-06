using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyCQRS.Azure.Migrations
{
    public partial class infrastructurev014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Sagas",
                table: "Sagas");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Sagas",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sagas",
                table: "Sagas",
                columns: new[] { "Id", "FullName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Sagas",
                table: "Sagas");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Sagas",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sagas",
                table: "Sagas",
                columns: new[] { "Id", "Type" });
        }
    }
}
