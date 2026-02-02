using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActionToViewTechnicianAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Technician
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 3, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_VIEW_MAIN_MENU_APPOINTMENTS')
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
