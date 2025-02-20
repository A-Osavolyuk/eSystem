using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenamedCodeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("05a05396-a493-44a1-b8ca-9ea87550bb17"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("0b975eac-8597-4690-8ebb-9c87f59ecbde"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                column: "ConcurrencyStamp",
                value: "8464a3ed-8df8-4bb2-acbe-fce5a39462d8");

            migrationBuilder.InsertData(
                table: "PersonalData",
                columns: new[] { "Id", "CreateDate", "DateOfBirth", "FirstName", "Gender", "LastName", "UpdateDate", "UserId" },
                values: new object[] { new Guid("05a05396-a493-44a1-b8ca-9ea87550bb17"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Male", "Osavolyuk", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3") });
        }
    }
}
