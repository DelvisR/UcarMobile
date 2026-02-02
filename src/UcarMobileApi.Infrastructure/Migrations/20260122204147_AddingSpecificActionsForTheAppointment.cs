using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSpecificActionsForTheAppointment : Migration
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
                { "ACTION_ALL_APPOINTMENTS", "Allows access to all appointments", "RESOURCE_DEFAULT" },
                { "ACTION_TECH_APPOINTMENTS", "Allows access to appointments belonging to the technician", "RESOURCE_DEFAULT" },
                { "ACTION_CLIENT_APPOINTMENTS", "Allows access to appointments belonging to the client", "RESOURCE_DEFAULT" }
            });

            // admins
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 1, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_ALL_APPOINTMENTS')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 1 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // Technician register device
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 3, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_TECH_APPOINTMENTS','ACTION_VIEW_MAIN_MENU_APPOINTMENTS')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 3 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // assign appointment actions to Client rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 6, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_CLIENT_APPOINTMENTS')
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
