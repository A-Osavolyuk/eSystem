using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveActionTypeFromPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
