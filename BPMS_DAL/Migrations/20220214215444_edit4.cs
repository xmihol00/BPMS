using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "PhoneNumber", "Surname", "UserName" },
                values: new object[] { new Guid("342c2de7-eb92-44f9-acf1-41d5dade854b"), "pavel@test.cz", "Pavel", "", "", "Svoboda", "paja" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "PhoneNumber", "Surname", "UserName" },
                values: new object[] { new Guid("6e250b64-ea22-4880-86d2-94d547b2e1b5"), "karel@test.cz", "Karel", "", "", "Stavitel", "kaja" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("342c2de7-eb92-44f9-acf1-41d5dade854b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6e250b64-ea22-4880-86d2-94d547b2e1b5"));
        }
    }
}
