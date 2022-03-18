using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("26ea2c26-f4c9-43b0-8607-f7de1dad9fcd"),
                column: "Key",
                value: new byte[] { 51, 255, 78, 181, 34, 125, 218, 30, 175, 231, 117, 17, 64, 175, 245, 163, 230, 97, 5, 161, 118, 34, 29, 135, 52, 187, 82, 147, 172, 241, 123, 255, 248, 59, 64, 11, 31, 29, 245, 61, 145, 141, 225, 140, 225, 181, 47, 117 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "ISaWTbZCvnHDDVXb5Y8YEIVJjBTbBqKHAu7XOgz2jnYEaUWBpiXZGI1Zzd7YB+aZnyE7CpPOIRN3kDG/92QrU7GX");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("26ea2c26-f4c9-43b0-8607-f7de1dad9fcd"),
                column: "Key",
                value: new byte[] { 50, 115, 53, 118, 56, 121, 47, 66, 63, 69, 40, 72, 43, 77, 98, 81, 101, 84, 104, 86, 109, 89, 113, 51, 116, 54, 119, 57, 122, 36, 67, 38 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "0D4Goyxhm3BVmcRZjILlAmmcYVXzA/r4F3EuTgPs9/IceLePOrWev0OuL3p5zMLN29kLXg8THR9DllAmtRugzG4P");
        }
    }
}
