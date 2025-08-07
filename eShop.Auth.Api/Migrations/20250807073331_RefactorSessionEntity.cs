using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSessionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "OAuthSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "ProviderId",
                table: "OAuthSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OAuthSessions_ProviderId",
                table: "OAuthSessions",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthSessions_UserId",
                table: "OAuthSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OAuthSessions_OAuthProviders_ProviderId",
                table: "OAuthSessions",
                column: "ProviderId",
                principalTable: "OAuthProviders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OAuthSessions_Users_UserId",
                table: "OAuthSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OAuthSessions_OAuthProviders_ProviderId",
                table: "OAuthSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_OAuthSessions_Users_UserId",
                table: "OAuthSessions");

            migrationBuilder.DropIndex(
                name: "IX_OAuthSessions_ProviderId",
                table: "OAuthSessions");

            migrationBuilder.DropIndex(
                name: "IX_OAuthSessions_UserId",
                table: "OAuthSessions");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "OAuthSessions");

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "OAuthSessions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
