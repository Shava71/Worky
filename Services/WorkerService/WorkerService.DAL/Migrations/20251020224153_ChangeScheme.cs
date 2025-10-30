using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkerService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "typeOfActivity_id",
                table: "Resume_filter",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "typeOfActivity_id",
                table: "Resume_filter",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
