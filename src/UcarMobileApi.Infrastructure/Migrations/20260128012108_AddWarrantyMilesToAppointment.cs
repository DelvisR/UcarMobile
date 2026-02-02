using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWarrantyMilesToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WarrantyMonths",
                table: "Appointment",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarrantyMiles",
                table: "Appointment",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarrantyMiles",
                table: "Appointment");

            migrationBuilder.AlterColumn<int>(
                name: "WarrantyMonths",
                table: "Appointment",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
