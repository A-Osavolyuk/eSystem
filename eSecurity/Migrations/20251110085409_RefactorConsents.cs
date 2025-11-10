using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSecurity.Migrations
{
    /// <inheritdoc />
    public partial class RefactorConsents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consents_UserId",
                table: "Consents");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UserId",
                table: "Consents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consents_UserId",
                table: "Consents");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UserId",
                table: "Consents",
                column: "UserId",
                unique: true);
        }
    }
}
