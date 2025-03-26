using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plant_Explorer.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_New_Plant_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Plants",
                newName: "Habitat");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "ScanHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Probability",
                table: "ScanHistories",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Distribution",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Family",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoritePlant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePlant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoritePlant_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoritePlant_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlantImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantImage_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlantApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantApplication_ApplicationCategory_ApplicationCategoryId",
                        column: x => x.ApplicationCategoryId,
                        principalTable: "ApplicationCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlantApplication_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlantCharacteristic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacteristicCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantCharacteristic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantCharacteristic_CharacteristicCategory_CharacteristicCategoryId",
                        column: x => x.CharacteristicCategoryId,
                        principalTable: "CharacteristicCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlantCharacteristic_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePlant_PlantId",
                table: "FavoritePlant",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePlant_UserId",
                table: "FavoritePlant",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantApplication_ApplicationCategoryId",
                table: "PlantApplication",
                column: "ApplicationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantApplication_PlantId",
                table: "PlantApplication",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCharacteristic_CharacteristicCategoryId",
                table: "PlantCharacteristic",
                column: "CharacteristicCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCharacteristic_PlantId",
                table: "PlantCharacteristic",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantImage_PlantId",
                table: "PlantImage",
                column: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritePlant");

            migrationBuilder.DropTable(
                name: "PlantApplication");

            migrationBuilder.DropTable(
                name: "PlantCharacteristic");

            migrationBuilder.DropTable(
                name: "PlantImage");

            migrationBuilder.DropTable(
                name: "ApplicationCategory");

            migrationBuilder.DropTable(
                name: "CharacteristicCategory");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "ScanHistories");

            migrationBuilder.DropColumn(
                name: "Probability",
                table: "ScanHistories");

            migrationBuilder.DropColumn(
                name: "Distribution",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Family",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "Habitat",
                table: "Plants",
                newName: "ImageUrl");
        }
    }
}
