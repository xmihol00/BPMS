using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_SolvingRoles_RoleId",
                table: "AgendaRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_Users_RoleId",
                table: "AgendaRoles");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AgendaRoles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "AgendaRoles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgendaId",
                table: "AgendaRoles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_SolvingRoles_RoleId",
                table: "AgendaRoles",
                column: "RoleId",
                principalTable: "SolvingRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_Users_RoleId",
                table: "AgendaRoles",
                column: "RoleId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_SolvingRoles_RoleId",
                table: "AgendaRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_Users_RoleId",
                table: "AgendaRoles");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AgendaRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "AgendaRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AgendaId",
                table: "AgendaRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_SolvingRoles_RoleId",
                table: "AgendaRoles",
                column: "RoleId",
                principalTable: "SolvingRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_Users_RoleId",
                table: "AgendaRoles",
                column: "RoleId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
