using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_Explorer.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAttributeToSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserPoint",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Plants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Options",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CharacteristicCategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BugReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Badges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Avatars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ApplicationCategory",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserPoint");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CharacteristicCategory");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BugReports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ApplicationCategory");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: true);
        }
    }
}
