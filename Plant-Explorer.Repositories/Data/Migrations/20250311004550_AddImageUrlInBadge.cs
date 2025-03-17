using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_Explorer.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlInBadge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "Badges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "Badges");
        }
    }
}
