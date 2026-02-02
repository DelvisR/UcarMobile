using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovingAppointmentPartsToAppointmentServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentPart_AppointmentVehicle_AppointmentVehicleId",
                table: "AppointmentPart");

            migrationBuilder.DropColumn(
                name: "OdometerKm",
                table: "ClientVehicle");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AppointmentService");

            migrationBuilder.RenameColumn(
                name: "AppointmentVehicleId",
                table: "AppointmentPart",
                newName: "AppointmentServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentPart_AppointmentVehicleId",
                table: "AppointmentPart",
                newName: "IX_AppointmentPart_AppointmentServiceId");

            migrationBuilder.AddColumn<int>(
                name: "OdometerKm",
                table: "AppointmentVehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentPart_AppointmentService_AppointmentServiceId",
                table: "AppointmentPart",
                column: "AppointmentServiceId",
                principalTable: "AppointmentService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"DELETE FROM ""AppointmentNoteDocument"";");
            migrationBuilder.Sql(@"DELETE FROM ""AppointmentNote"";");
            migrationBuilder.Sql(@"DELETE FROM ""AppointmentDocument"";");
            migrationBuilder.Sql(@"DELETE FROM ""Appointment"";");

            migrationBuilder.DeleteData(table: "ServiceCategory", keyColumn: "Id", keyValue: 10);
            migrationBuilder.DeleteData(table: "ServiceCategory", keyColumn: "Id", keyValue: 11);

            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Value", "ValueType", "Description"],
                values: new object[,]
                {
                    {
                        "WARRANTY_MILES", "12000", "int",
                        "Warranty coverage in miles for the completed repair or service"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentPart_AppointmentService_AppointmentServiceId",
                table: "AppointmentPart");

            migrationBuilder.DropColumn(
                name: "OdometerKm",
                table: "AppointmentVehicle");

            migrationBuilder.RenameColumn(
                name: "AppointmentServiceId",
                table: "AppointmentPart",
                newName: "AppointmentVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentPart_AppointmentServiceId",
                table: "AppointmentPart",
                newName: "IX_AppointmentPart_AppointmentVehicleId");

            migrationBuilder.AddColumn<int>(
                name: "OdometerKm",
                table: "ClientVehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AppointmentService",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentPart_AppointmentVehicle_AppointmentVehicleId",
                table: "AppointmentPart",
                column: "AppointmentVehicleId",
                principalTable: "AppointmentVehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
