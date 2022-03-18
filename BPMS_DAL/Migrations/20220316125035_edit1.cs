using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("23bdf844-8587-4ccb-92c3-66513ade014a"),
                column: "URL",
                value: "TODO/EchoJson");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("45adf844-8587-4bbb-92c3-66513ade014a"),
                column: "URL",
                value: "TODO/EchoXml");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("45adf844-8587-4ffb-92c3-66513ade014a"),
                column: "URL",
                value: "TODO/EchoXml");

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "AppId", "AppSecret", "AuthType", "Description", "HttpMethod", "Name", "Serialization", "Type", "URL" },
                values: new object[] { new Guid("45adf844-8587-4ffb-92c3-99a23ade014a"), "MyUserName", "LongAndSecurePassword", 1, "Ozvěna vstupu serializovaného do XML tagů.", 2, "Echo API - Hlavičky", 1, 0, "TODO/EchoHeaders" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "p7vltOmUFfyIlgoLmpz5Mp7MIuiX/NB5Bo/Mu1yVcnUQ1jAB9yjnTegD6PqGlmLUjLetUhWd2gzaNaR5IOovqm68");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("45adf844-8587-4ffb-92c3-99a23ade014a"));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("23bdf844-8587-4ccb-92c3-66513ade014a"),
                column: "URL",
                value: "TODO");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("45adf844-8587-4bbb-92c3-66513ade014a"),
                column: "URL",
                value: "TODO");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("45adf844-8587-4ffb-92c3-66513ade014a"),
                column: "URL",
                value: "TODO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "efBAtZwnw28i7emXw2MYnAWzZXMnRmJ7fLWV03DuTnI3USyKbPxRhvXtC1AGqAsfL29kWjQTcVEc2e0iNXW4npXQ");
        }
    }
}
