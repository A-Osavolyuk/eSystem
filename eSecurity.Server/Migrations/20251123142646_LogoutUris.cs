using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSecurity.Server.Migrations
{
    /// <inheritdoc />
    public partial class LogoutUris : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Clients",
                newName: "ClientType");

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenType",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ClientBackChannelLogoutUris",
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
                    table.PrimaryKey("PK_ClientBackChannelLogoutUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientBackChannelLogoutUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientFrontChannelLogoutUris",
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
                    table.PrimaryKey("PK_ClientFrontChannelLogoutUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientFrontChannelLogoutUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpaqueTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Revoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpaqueTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpaqueTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpaqueTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpaqueTokensScopes",
                columns: table => new
                {
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpaqueTokensScopes", x => new { x.TokenId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_OpaqueTokensScopes_OpaqueTokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "OpaqueTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpaqueTokensScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientBackChannelLogoutUris_ClientId",
                table: "ClientBackChannelLogoutUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientFrontChannelLogoutUris_ClientId",
                table: "ClientFrontChannelLogoutUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokens_ClientId",
                table: "OpaqueTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokens_UserId",
                table: "OpaqueTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokensScopes_ScopeId",
                table: "OpaqueTokensScopes",
                column: "ScopeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientBackChannelLogoutUris");

            migrationBuilder.DropTable(
                name: "ClientFrontChannelLogoutUris");

            migrationBuilder.DropTable(
                name: "OpaqueTokensScopes");

            migrationBuilder.DropTable(
                name: "OpaqueTokens");

            migrationBuilder.DropColumn(
                name: "AccessTokenType",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ClientType",
                table: "Clients",
                newName: "Type");
        }
    }
}
