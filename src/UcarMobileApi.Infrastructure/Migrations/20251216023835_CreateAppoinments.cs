using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateAppoinments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientVehicle_ClientId",
                table: "ClientVehicle");

            migrationBuilder.DropIndex(
                name: "IX_ClientVehicle_ClientId_VehicleId",
                table: "ClientVehicle");

            migrationBuilder.DropIndex(
                name: "IX_ClientVehicle_LicensePlate",
                table: "ClientVehicle");

            migrationBuilder.RenameColumn(
                name: "BaseAddress_BasePoint",
                table: "Technician",
                newName: "BasePoint");

            migrationBuilder.RenameIndex(
                name: "IX_Technician_BaseAddress_BasePoint",
                table: "Technician",
                newName: "IX_Technician_BasePoint");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "AppointmentService",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "AppointmentPart",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "ServiceAddress_BasePoint",
                table: "Appointment",
                newName: "BasePoint");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_ServiceAddress_BasePoint",
                table: "Appointment",
                newName: "IX_Appointment_BasePoint");

            migrationBuilder.AddColumn<int>(
                name: "AzId",
                table: "ClientVehicle",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "AppointmentVehicle",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "CustomService",
                table: "AppointmentService",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_ClientId_VehicleId_AzId",
                table: "ClientVehicle",
                columns: new[] { "ClientId", "VehicleId", "AzId" },
                unique: true,
                filter: "\"AzId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientVehicle_ClientId_VehicleId_AzId",
                table: "ClientVehicle");

            migrationBuilder.DropColumn(
                name: "AzId",
                table: "ClientVehicle");

            migrationBuilder.DropColumn(
                name: "CustomService",
                table: "AppointmentService");

            migrationBuilder.RenameColumn(
                name: "BasePoint",
                table: "Technician",
                newName: "BaseAddress_BasePoint");

            migrationBuilder.RenameIndex(
                name: "IX_Technician_BasePoint",
                table: "Technician",
                newName: "IX_Technician_BaseAddress_BasePoint");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "AppointmentService",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "AppointmentPart",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "BasePoint",
                table: "Appointment",
                newName: "ServiceAddress_BasePoint");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_BasePoint",
                table: "Appointment",
                newName: "IX_Appointment_ServiceAddress_BasePoint");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "AppointmentVehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_ClientId",
                table: "ClientVehicle",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_ClientId_VehicleId",
                table: "ClientVehicle",
                columns: new[] { "ClientId", "VehicleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_LicensePlate",
                table: "ClientVehicle",
                column: "LicensePlate");
        }
    }
}
