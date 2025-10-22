using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureOAuthSessionCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OAuthSessions_UserOAuthProviders_LinkedAccountId",
                table: "OAuthSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_OAuthSessions_UserOAuthProviders_LinkedAccountId",
                table: "OAuthSessions",
                column: "LinkedAccountId",
                principalTable: "UserOAuthProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OAuthSessions_UserOAuthProviders_LinkedAccountId",
                table: "OAuthSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_OAuthSessions_UserOAuthProviders_LinkedAccountId",
                table: "OAuthSessions",
                column: "LinkedAccountId",
                principalTable: "UserOAuthProviders",
                principalColumn: "Id");
        }
    }
}
