using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSecurity.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectTypeConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Secret",
                table: "Clients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "SectorIdentifierUri",
                table: "Clients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectType",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectorIdentifierUri",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "Secret",
                table: "Clients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
