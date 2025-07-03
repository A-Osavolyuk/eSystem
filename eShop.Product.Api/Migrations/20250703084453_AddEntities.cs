using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Product.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Berries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Variety = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RipenessStage = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StorageTemperature = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RequiresRefrigeration = table.Column<bool>(type: "bit", nullable: false),
                    ContainsSeeds = table.Column<bool>(type: "bit", nullable: false),
                    IsWildHarvested = table.Column<bool>(type: "bit", nullable: false),
                    IsOrganic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Berries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Berries_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vegetables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Variety = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RipenessStage = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StorageTemperature = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RequiresRefrigeration = table.Column<bool>(type: "bit", nullable: false),
                    IsLeafy = table.Column<bool>(type: "bit", nullable: false),
                    IsRootVegetable = table.Column<bool>(type: "bit", nullable: false),
                    IsOrganic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vegetables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vegetables_Products_Id",
                        column: x => x.Id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Berries");

            migrationBuilder.DropTable(
                name: "Vegetables");
        }
    }
}
