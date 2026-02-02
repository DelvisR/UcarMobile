using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateAppoinmentsActions : Migration
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
                { "ACTION_VIEW_MAIN_MENU_APPOINTMENTS", "Appointments menu action", "RESOURCE_MAIN_MENU" },
                { "ACTION_CREATE_APPOINTMENT", "Allows creating appointments", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_APPOINTMENT", "Allows editing appointments", "RESOURCE_DEFAULT" }
            });

            // Technician register device	
            migrationBuilder.InsertData(
                table: "RoleAction",
                columns: ["RoleId", "ActionId"],
                values: new object[] { 3, 14 }
            );

            // assign appointment actions to Client rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 6, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_VIEW_MAIN_MENU_APPOINTMENTS', 'ACTION_CREATE_APPOINTMENT', 'ACTION_EDIT_APPOINTMENT')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 6 AND ra.""ActionId"" = a.""Id""
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
