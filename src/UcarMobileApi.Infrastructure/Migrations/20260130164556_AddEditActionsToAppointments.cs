using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEditActionsToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Action",
            columns:
            ["Name", "Description", "Resource"],
            values: new object[,]
            {
                { "ACTION_EDIT_APPOINTMENT_LOCATION", "Allows editing the location of appointment", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_RESCHEDULE", "Allows rescheduling an appointment", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_NOTE", "Allows editing notes for an appointment", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_PAYMENT", "Allows editing payment details for an appointment", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_CANCEL", "Allows canceling an appointment", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_FINALIZE", "Allows finalizing the appointment by completing, closing and triggering the billing process", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_SERVICES", "Allows edit the services and parts for an appointment", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT_TECHNICIAN", "Allows editing or reassigning the technician for an appointment", "RESOURCE_DEFAULT" },
                { "ACTION_ADD_APPOINTMENT_NOTE", "Allows adding new notes to an appointment", "RESOURCE_DEFAULT" }
            });

            // assign appointment actions to Client rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 6, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_EDIT_APPOINTMENT_LOCATION', 'ACTION_EDIT_APPOINTMENT_RESCHEDULE', 'ACTION_EDIT_APPOINTMENT_NOTE', 'ACTION_EDIT_APPOINTMENT_PAYMENT', 'ACTION_EDIT_APPOINTMENT_CANCEL')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 6 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // assign appointment actions to Admin rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 1, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_EDIT_APPOINTMENT_FINALIZE', 'ACTION_EDIT_APPOINTMENT_LOCATION', 'ACTION_EDIT_APPOINTMENT_RESCHEDULE', 'ACTION_EDIT_APPOINTMENT_NOTE', 'ACTION_EDIT_APPOINTMENT_PAYMENT', 'ACTION_EDIT_APPOINTMENT_CANCEL', 'ACTION_EDIT_APPOINTMENT_SERVICES', 'ACTION_EDIT_APPOINTMENT_TECHNICIAN', 'ACTION_ADD_APPOINTMENT_NOTE')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 1 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // assign appointment actions to tech rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 3, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_ADD_APPOINTMENT_NOTE','ACTION_EDIT_APPOINTMENT_FINALIZE')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 3 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Category", "Value", "ValueType", "Description", "Order"],
                values: new object[,]
                {
                    {
                        "CANCELLATION_FEE_AMOUNT", "Client & Booking Management", "90", "decimal",
                        "Fixed fee charged to the customer when a scheduled repair appointment is canceled outside the allowed cancellation window", 131
                    },
                    {
                        "CANCELLATION_MIN_SERVICE_FEE_AMOUNT", "Client & Booking Management", "90", "decimal",
                        "Fixed amount charged as the minimum service fee when the customer cancels the appointment while the technician is en route or already on site", 132
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
