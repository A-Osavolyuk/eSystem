using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRemoveRedundantProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "OAuthSessions");

            migrationBuilder.DropColumn(
                name: "ErrorType",
                table: "OAuthSessions");

            migrationBuilder.DropColumn(
                name: "IsSucceeded",
                table: "OAuthSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "OAuthSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorType",
                table: "OAuthSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSucceeded",
                table: "OAuthSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
