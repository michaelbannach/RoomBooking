using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomBookingC.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyinEmployeeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Employees",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Employees",
                newName: "Surname");
        }
    }
}
