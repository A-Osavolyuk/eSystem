using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("cf9568ee-c96b-4d39-ad63-d39f989b8b22"));

            migrationBuilder.RenameColumn(
                name: "ExpiredAt",
                table: "SecurityTokens",
                newName: "UpdateDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "SecurityTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "SecurityTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                column: "ConcurrencyStamp",
                value: "c3daa8d3-35ee-49f3-b925-063a47983ece");

            migrationBuilder.InsertData(
                table: "PersonalData",
                columns: new[] { "Id", "CreateDate", "DateOfBirth", "FirstName", "Gender", "LastName", "UpdateDate", "UserId" },
                values: new object[] { new Guid("c6a745cb-4705-4572-959a-afe0200c9586"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Male", "Osavolyuk", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("c6a745cb-4705-4572-959a-afe0200c9586"));

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "SecurityTokens");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "SecurityTokens");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "SecurityTokens",
                newName: "ExpiredAt");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                column: "ConcurrencyStamp",
                value: "a67cda70-6003-4c25-b48d-5edde62c852b");

            migrationBuilder.InsertData(
                table: "PersonalData",
                columns: new[] { "Id", "CreateDate", "DateOfBirth", "FirstName", "Gender", "LastName", "UpdateDate", "UserId" },
                values: new object[] { new Guid("cf9568ee-c96b-4d39-ad63-d39f989b8b22"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Male", "Osavolyuk", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3") });
        }
    }
}
