using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribedDate",
                table: "UserProvider");

            migrationBuilder.DropColumn(
                name: "UnsubscribedDate",
                table: "UserProvider");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SubscribedDate",
                table: "UserProvider",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UnsubscribedDate",
                table: "UserProvider",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
