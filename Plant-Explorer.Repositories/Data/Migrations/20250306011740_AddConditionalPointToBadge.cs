using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_Explorer.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConditionalPointToBadge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "conditionalPoint",
                table: "Badges",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "conditionalPoint",
                table: "Badges");
        }
    }
}
