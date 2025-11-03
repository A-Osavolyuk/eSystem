using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSystem.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClientPostLogoutEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RedirectUri",
                table: "ClientRedirectUris",
                newName: "Uri");

            migrationBuilder.CreateTable(
                name: "ClientPostLogoutUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPostLogoutUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPostLogoutUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientPostLogoutUris_ClientId",
                table: "ClientPostLogoutUris",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPostLogoutUris");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "ClientRedirectUris",
                newName: "RedirectUri");
        }
    }
}
