using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryDate",
                table: "UserPhoneNumbers");

            migrationBuilder.DropColumn(
                name: "PrimaryDate",
                table: "UserEmails");

            migrationBuilder.DropColumn(
                name: "RecoveryDate",
                table: "UserEmails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PrimaryDate",
                table: "UserPhoneNumbers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PrimaryDate",
                table: "UserEmails",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RecoveryDate",
                table: "UserEmails",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
