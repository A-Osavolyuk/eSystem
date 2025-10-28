using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSystem.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExtendRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshDate",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "NewTokenId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RevokeDate",
                table: "RefreshTokens",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_NewTokenId",
                table: "RefreshTokens",
                column: "NewTokenId",
                unique: true,
                filter: "[NewTokenId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Clients_NewTokenId",
                table: "RefreshTokens",
                column: "NewTokenId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Clients_NewTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_NewTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "NewTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokeDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RefreshDate",
                table: "RefreshTokens",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
