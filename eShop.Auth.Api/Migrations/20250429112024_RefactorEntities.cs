using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("c6a745cb-4705-4572-959a-afe0200c9586"));

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Codes");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Codes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                column: "ConcurrencyStamp",
                value: "3e6a6ed2-f20d-44ce-bb53-57abc09d4325");

            migrationBuilder.InsertData(
                table: "PersonalData",
                columns: new[] { "Id", "CreateDate", "DateOfBirth", "FirstName", "Gender", "LastName", "UpdateDate", "UserId" },
                values: new object[] { new Guid("682b5578-c6fa-4b25-9ff0-ae3f3bb5cab2"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alexander", "Male", "Osavolyuk", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3") });

            migrationBuilder.CreateIndex(
                name: "IX_Codes_UserId",
                table: "Codes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_AspNetUsers_UserId",
                table: "Codes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Codes_AspNetUsers_UserId",
                table: "Codes");

            migrationBuilder.DropIndex(
                name: "IX_Codes_UserId",
                table: "Codes");

            migrationBuilder.DeleteData(
                table: "PersonalData",
                keyColumn: "Id",
                keyValue: new Guid("682b5578-c6fa-4b25-9ff0-ae3f3bb5cab2"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Codes");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Codes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
    }
}
