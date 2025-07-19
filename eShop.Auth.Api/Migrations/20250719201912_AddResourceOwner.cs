using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ResourceOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceOwners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_OwnerId",
                table: "Resources",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_ResourceOwners_OwnerId",
                table: "Resources",
                column: "OwnerId",
                principalTable: "ResourceOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_ResourceOwners_OwnerId",
                table: "Resources");

            migrationBuilder.DropTable(
                name: "ResourceOwners");

            migrationBuilder.DropIndex(
                name: "IX_Resources_OwnerId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Resources");
        }
    }
}
