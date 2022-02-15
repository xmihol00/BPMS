using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class edit7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Models");
        }
    }
}
