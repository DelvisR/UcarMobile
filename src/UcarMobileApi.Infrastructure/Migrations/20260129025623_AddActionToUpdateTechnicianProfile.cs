using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActionToUpdateTechnicianProfile : Migration
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
                { "ACTION_GET_TECHNICIAN_PROFILE", "Allows a technician to get their profile information", "RESOURCE_DEFAULT" },
                { "ACTION_UPDATE_TECHNICIAN_PROFILE", "Allows a technician to update their profile information", "RESOURCE_DEFAULT" }
            });

            // admins
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 1, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_GET_TECHNICIAN_PROFILE', 'ACTION_UPDATE_TECHNICIAN_PROFILE')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 1 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // Technician
            migrationBuilder.Sql(@"
                DELETE FROM ""RoleAction""
                WHERE ""RoleId"" = 3
                  AND ""ActionId"" IN (
                      SELECT ""Id"" FROM ""Action""
                      WHERE ""Name"" IN ('ACTION_VIEW_MAIN_MENU_APPOINTMENTS')
                  );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 3, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_GET_TECHNICIAN_PROFILE', 'ACTION_UPDATE_TECHNICIAN_PROFILE')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 3 AND ra.""ActionId"" = a.""Id""
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
