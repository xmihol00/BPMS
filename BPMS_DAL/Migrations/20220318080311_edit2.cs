using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForeignAttributeMaps_Attributes_AttributeId",
                table: "ForeignAttributeMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_ForeignAttributeMaps_ForeignSendEvents_ForeignSendEventId",
                table: "ForeignAttributeMaps");

            migrationBuilder.AddColumn<int>(
                name: "Encryption",
                table: "Systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ForeignEncryption",
                table: "Systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "DataSchemas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "Attributes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "D2F3F9+WGzr2BC+RdpUyNWwJYO3LPrVfyMBUI3yYI2Fc8FmGV8naBgfXQjXJ5ZIpWxqq8j/jkiMx2QhJC/DEiykj");

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignAttributeMaps_Attributes_AttributeId",
                table: "ForeignAttributeMaps",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignAttributeMaps_ForeignSendEvents_ForeignSendEventId",
                table: "ForeignAttributeMaps",
                column: "ForeignSendEventId",
                principalTable: "ForeignSendEvents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForeignAttributeMaps_Attributes_AttributeId",
                table: "ForeignAttributeMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_ForeignAttributeMaps_ForeignSendEvents_ForeignSendEventId",
                table: "ForeignAttributeMaps");

            migrationBuilder.DropColumn(
                name: "Encryption",
                table: "Systems");

            migrationBuilder.DropColumn(
                name: "ForeignEncryption",
                table: "Systems");

            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "DataSchemas");

            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "Attributes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                column: "Password",
                value: "p7vltOmUFfyIlgoLmpz5Mp7MIuiX/NB5Bo/Mu1yVcnUQ1jAB9yjnTegD6PqGlmLUjLetUhWd2gzaNaR5IOovqm68");

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignAttributeMaps_Attributes_AttributeId",
                table: "ForeignAttributeMaps",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignAttributeMaps_ForeignSendEvents_ForeignSendEventId",
                table: "ForeignAttributeMaps",
                column: "ForeignSendEventId",
                principalTable: "ForeignSendEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
