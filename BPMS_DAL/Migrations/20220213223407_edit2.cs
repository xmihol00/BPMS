using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_Agendas_RoleId",
                table: "AgendaRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_Users_RoleId",
                table: "AgendaRoles");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaRoles_AgendaId",
                table: "AgendaRoles",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaRoles_UserId",
                table: "AgendaRoles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_Agendas_AgendaId",
                table: "AgendaRoles",
                column: "AgendaId",
                principalTable: "Agendas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_Users_UserId",
                table: "AgendaRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_Agendas_AgendaId",
                table: "AgendaRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaRoles_Users_UserId",
                table: "AgendaRoles");

            migrationBuilder.DropIndex(
                name: "IX_AgendaRoles_AgendaId",
                table: "AgendaRoles");

            migrationBuilder.DropIndex(
                name: "IX_AgendaRoles_UserId",
                table: "AgendaRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_Agendas_RoleId",
                table: "AgendaRoles",
                column: "RoleId",
                principalTable: "Agendas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaRoles_Users_RoleId",
                table: "AgendaRoles",
                column: "RoleId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
