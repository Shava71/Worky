using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FilterService.Migrations
{
    /// <inheritdoc />
    public partial class AddEducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "Educations",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Начальное общее образование" },
                    { 2, "Основное общее образование" },
                    { 3, "Среднее общее образование" },
                    { 4, "Среднее профессиональное образование" },
                    { 5, "Бакалавриат" },
                    { 6, "Специалитет" },
                    { 7, "Магистратура" },
                    { 8, "Аспирантура" },
                    { 9, "Ординатура" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Educations");
        }
    }
}
