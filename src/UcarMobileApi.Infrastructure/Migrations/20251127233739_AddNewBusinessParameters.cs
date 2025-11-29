using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewBusinessParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing value
            migrationBuilder.UpdateData(
                table: "BusinessParameter",
                keyColumn: "Key",
                keyValue: "DefaultClientRoles",
                columns: ["Key", "Description"],
                values: new object[] { "DEFAULT_CLIENT_ROLES", "Default roles for new clients" }
            );

            // Insert new parameters
            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Value", "ValueType", "Description"],
                values: new object[,]
                {
                    {
                        "DIAGNOSTIC_WINDOW_MINUTES", "60", "int",
                        "Default diagnostic window duration in hours when only one technician is available"
                    },
                    {
                        "REPAIR_BUFFER_HOURS", "4", "decimal",
                        "Automatic buffer time in hours allocated for potential repairs after diagnosis"
                    },
                    {
                        "BOOKING_CALENDAR_DAYS_AHEAD", "30", "int",
                        "Number of days ahead to display in the booking calendar for clients"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
