using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "DataSchemas");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DataSchemas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "0D4Goyxhm3BVmcRZjILlAmmcYVXzA/r4F3EuTgPs9/IceLePOrWev0OuL3p5zMLN29kLXg8THR9DllAmtRugzG4P");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DataSchemas");

            migrationBuilder.AddColumn<long>(
                name: "Order",
                table: "DataSchemas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "8c7qaxIit0oWzK7FGoDbgs+Sh0KkDe2ZJy7QjGaOIiTA5/szc9ZOnlD020sfbXTy8aFJ8pUYfpXc2wHi/JLc3O0z");
        }
    }
}
