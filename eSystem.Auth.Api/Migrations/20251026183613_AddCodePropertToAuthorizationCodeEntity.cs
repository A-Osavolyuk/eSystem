using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSystem.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCodePropertToAuthorizationCodeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AuthorizationCodes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "AuthorizationCodes");
        }
    }
}
