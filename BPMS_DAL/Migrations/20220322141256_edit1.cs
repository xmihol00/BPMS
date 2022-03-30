using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedEnd",
                table: "Workflows",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "MX/BSM2/Zy5Bfxl7a2TFUXqKIUVgCGEcCl7iLaxFZugrCc7Slg7nNRIspJaPH29MJQONWNgOxq32DxvfJ/cOothF");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedEnd",
                table: "Workflows");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "7nfgJv29qmTR78+wRQRxMaRKow6Il6DqYDkQjVyNIMe46lX9ZO1Cwc5NRcw+k+rdAJo2yODgKx1kFmanrsDeyi+l");
        }
    }
}
