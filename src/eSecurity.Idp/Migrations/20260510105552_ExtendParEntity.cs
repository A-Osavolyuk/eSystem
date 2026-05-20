using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSecurity.Idp.Migrations
{
    /// <inheritdoc />
    public partial class ExtendParEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestUri",
                schema: "public",
                table: "PushedAuthorizationRequest",
                type: "character varying(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestUri",
                schema: "public",
                table: "PushedAuthorizationRequest");
        }
    }
}
