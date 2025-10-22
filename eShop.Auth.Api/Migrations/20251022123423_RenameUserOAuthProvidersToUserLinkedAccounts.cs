using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserOAuthProvidersToUserLinkedAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OAuthSessions_UserOAuthProviders_LinkedAccountId",
                table: "OAuthSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOAuthProviders_Users_UserId",
                table: "UserOAuthProviders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserOAuthProviders",
                table: "UserOAuthProviders");

            migrationBuilder.RenameTable(
                name: "UserOAuthProviders",
                newName: "UserLinkedAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_UserOAuthProviders_UserId",
                table: "UserLinkedAccounts",
                newName: "IX_UserLinkedAccounts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLinkedAccounts",
                table: "UserLinkedAccounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OAuthSessions_UserLinkedAccounts_LinkedAccountId",
                table: "OAuthSessions",
                column: "LinkedAccountId",
                principalTable: "UserLinkedAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLinkedAccounts_Users_UserId",
                table: "UserLinkedAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OAuthSessions_UserLinkedAccounts_LinkedAccountId",
                table: "OAuthSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLinkedAccounts_Users_UserId",
                table: "UserLinkedAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLinkedAccounts",
                table: "UserLinkedAccounts");

            migrationBuilder.RenameTable(
                name: "UserLinkedAccounts",
                newName: "UserOAuthProviders");

            migrationBuilder.RenameIndex(
                name: "IX_UserLinkedAccounts_UserId",
                table: "UserOAuthProviders",
                newName: "IX_UserOAuthProviders_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserOAuthProviders",
                table: "UserOAuthProviders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OAuthSessions_UserOAuthProviders_LinkedAccountId",
                table: "OAuthSessions",
                column: "LinkedAccountId",
                principalTable: "UserOAuthProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOAuthProviders_Users_UserId",
                table: "UserOAuthProviders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
