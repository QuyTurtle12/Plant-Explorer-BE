using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_Explorer.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixImageInQuizAndQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "Questions");
        }
    }
}
