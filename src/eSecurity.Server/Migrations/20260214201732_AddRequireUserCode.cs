using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSecurity.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddRequireUserCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireUserCode",
                schema: "public",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireUserCode",
                schema: "public",
                table: "Clients");
        }
    }
}
