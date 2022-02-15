using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Systems",
                columns: new[] { "Id", "Key", "Name", "ObtainedKey", "ObtainedName", "URL", "UniqueName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "", "Tento systém", "", "", "", "" },
                    { new Guid("87e156a0-6762-42b1-b67e-af105ed9a811"), "ABCD123456", "Test systém 3", "EFGH789456", "sys3", "https://localhost:5012/", "system3" },
                    { new Guid("90074a51-95a3-48a9-be3a-93b8ad3109d6"), "ABCD789456", "Test systém 1", "EFGH123456", "sys1", "https://localhost:5010/", "system1" },
                    { new Guid("ac0706e2-c282-49e0-99c7-5322e3235e62"), "789456ABCD", "Test systém 2", "123456EFGH", "sys2", "https://localhost:5011/", "system2" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("87e156a0-6762-42b1-b67e-af105ed9a811"));

            migrationBuilder.DeleteData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("90074a51-95a3-48a9-be3a-93b8ad3109d6"));

            migrationBuilder.DeleteData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ac0706e2-c282-49e0-99c7-5322e3235e62"));
        }
    }
}
