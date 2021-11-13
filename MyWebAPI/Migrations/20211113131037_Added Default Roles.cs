using Microsoft.EntityFrameworkCore.Migrations;

namespace MyWebAPI.Migrations
{
    public partial class AddedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bbacfc61-ac96-44cd-8ba2-6e619b3dd03f", "42c3313a-31ec-4484-a883-2820ccca9a0f", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6c09b0e2-12ec-453b-a931-0eabad34ff70", "e953c0a5-635b-4c25-81fe-dac302aa181f", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4a41643e-d1f6-4a45-8b98-7ca255c1bc4d", "8fa39a7c-2e77-4134-b1ad-e628179e8b7b", "SuperAdmin", "SUPERADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a41643e-d1f6-4a45-8b98-7ca255c1bc4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c09b0e2-12ec-453b-a931-0eabad34ff70");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbacfc61-ac96-44cd-8ba2-6e619b3dd03f");
        }
    }
}
