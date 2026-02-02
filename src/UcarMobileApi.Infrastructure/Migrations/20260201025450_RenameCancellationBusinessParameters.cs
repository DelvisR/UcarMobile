using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCancellationBusinessParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "CANCELLATION_FEE_AMOUNT", columns: ["Key", "Description"],
                values: ["APPOINTMENT_ADJUSTMENT_FEE_AMOUNT", "Fixed fee charged to the customer when an appointment is modified (cancellation, reschedule, address change, or other adjustments) outside the allowed adjustment window"]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "CANCELLATION_MIN_SERVICE_FEE_AMOUNT", columns: ["Key", "Description"],
                values: ["APPOINTMENT_MIN_SERVICE_FEE_AMOUNT", "Minimum service fee charged when the technician is en route or already on site and the appointment is modified, to cover incurred travel or labor costs"]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "CANCELLATION_WINDOW_HOURS", columns: ["Key", "Description"],
                values: ["APPOINTMENT_ADJUSTMENT_WINDOW_HOURS", "Number of hours before the scheduled service during which appointment modifications are allowed without penalty"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
