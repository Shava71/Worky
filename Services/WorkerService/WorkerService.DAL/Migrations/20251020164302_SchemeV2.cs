using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkerService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SchemeV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resume_Education_educationid",
                table: "Resume");

            migrationBuilder.DropIndex(
                name: "IX_Resume_educationid",
                table: "Resume");

            migrationBuilder.DropColumn(
                name: "educationid",
                table: "Resume");

            migrationBuilder.AlterColumn<int>(
                name: "experience",
                table: "Resume",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "education_id",
                table: "Resume",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resume_education_id",
                table: "Resume",
                column: "education_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resume_Education_education_id",
                table: "Resume",
                column: "education_id",
                principalTable: "Education",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resume_Education_education_id",
                table: "Resume");

            migrationBuilder.DropIndex(
                name: "IX_Resume_education_id",
                table: "Resume");

            migrationBuilder.AlterColumn<short>(
                name: "experience",
                table: "Resume",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "education_id",
                table: "Resume",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "educationid",
                table: "Resume",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resume_educationid",
                table: "Resume",
                column: "educationid");

            migrationBuilder.AddForeignKey(
                name: "FK_Resume_Education_educationid",
                table: "Resume",
                column: "educationid",
                principalTable: "Education",
                principalColumn: "id");
        }
    }
}
