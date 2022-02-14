using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "ServiceTasksModel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasksModel_RoleId",
                table: "ServiceTasksModel",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTasksModel_SolvingRoles_RoleId",
                table: "ServiceTasksModel",
                column: "RoleId",
                principalTable: "SolvingRoles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTasksModel_SolvingRoles_RoleId",
                table: "ServiceTasksModel");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTasksModel_RoleId",
                table: "ServiceTasksModel");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ServiceTasksModel");
        }
    }
}
