using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastSeenProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenAtUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25801C14-CBA0-4E74-8F6A-9AA57BA5A57F",
                column: "ConcurrencyStamp",
                value: "02aa3f3a-8d0a-49d4-8b16-1b5b18648e9a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BE3B9D48-68F5-42E3-9371-E7964F96A25D",
                column: "ConcurrencyStamp",
                value: "91d1da2c-4b4e-43c9-b764-e05de9c94149");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeenAtUtc",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25801C14-CBA0-4E74-8F6A-9AA57BA5A57F",
                column: "ConcurrencyStamp",
                value: "1c08fdae-46ca-4976-a8b1-279fe7a543f9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BE3B9D48-68F5-42E3-9371-E7964F96A25D",
                column: "ConcurrencyStamp",
                value: "4cbf25bd-5978-4be3-8bdd-80667116d550");
        }
    }
}
