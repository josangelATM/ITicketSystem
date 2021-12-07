using Microsoft.EntityFrameworkCore.Migrations;

namespace ITicketSystem.Data.Migrations
{
    public partial class AddTicketAssignedCountToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketAssignedCount",
                table: "AspNetUsers",
                type: "int",
                nullable: true,defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketAssignedCount",
                table: "AspNetUsers");
        }
    }
}
