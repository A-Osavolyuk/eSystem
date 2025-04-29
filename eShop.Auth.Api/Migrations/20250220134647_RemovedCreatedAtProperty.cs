using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCreatedAtProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("0b975eac-8597-4690-8ebb-9c87f59ecbde"));

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Codes");

            migrationBuilder.RenameColumn(
                name: "VerificationCodeType",
                table: "Codes",
                newName: "Type");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("cf9568ee-c96b-4d39-ad63-d39f989b8b22"));

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Codes",
                newName: "VerificationCodeType");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Codes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                column: "ConcurrencyStamp",
                value: "0b97f550-2520-4f7f-a7c8-83e7f675c702");

            migrationBuilder.InsertData(
                table: "PersonalData",
                columns: new[] { "Id", "CreateDate", "DateOfBirth", "FirstName", "Gender", "LastName", "UpdateDate", "UserId" },
                values: new object[] { new Guid("0b975eac-8597-4690-8ebb-9c87f59ecbde"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Male", "Osavolyuk", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3") });
        }
    }
}
