using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MintPlayer.Data.Migrations
{
    public partial class AddTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Subjects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedAt",
                table: "Subjects",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Subjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "InsertedAt",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Subjects");
        }
    }
}
