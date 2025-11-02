using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRolesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2e36db7b-412b-484c-b5ee-ccd3ab9c380c"), "Worker" },
                    { new Guid("6b8a3e9c-b44e-42fa-bc57-cc7a4ef961c0"), "Company" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("2e36db7b-412b-484c-b5ee-ccd3ab9c380c"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("6b8a3e9c-b44e-42fa-bc57-cc7a4ef961c0"));
        }
    }
}
