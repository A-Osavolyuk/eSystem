using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthenticatorId",
                table: "UserPasskeys",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticatorId",
                table: "UserPasskeys");
        }
    }
}
