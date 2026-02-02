using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientVehicleActions : Migration
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
                { "ACTION_VIEW_MENU_CLIENT_VEHICLES", "Client vehicles menu action", "RESOURCE_MAIN_MENU" },
                { "ACTION_CREATE_CLIENT_VEHICLE", "Allows creating Client vehicles", "RESOURCE_DEFAULT" },
                { "ACTION_EDIT_CLIENT_VEHICLE", "Allows editing Client vehicles", "RESOURCE_DEFAULT" },
                { "ACTION_DELETE_CLIENT_VEHICLE", "Allows delete Client vehicles", "RESOURCE_DEFAULT" }
            });

            // assign client actions to Client rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 6, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_VIEW_MENU_CLIENT_VEHICLES', 'ACTION_CREATE_CLIENT_VEHICLE', 'ACTION_EDIT_CLIENT_VEHICLE', 'ACTION_DELETE_CLIENT_VEHICLE')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 6 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // assign all acction to SuperAdmin rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 1, a.""Id"" FROM ""Action"" a
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""RoleAction"" ra 
                    WHERE ra.""RoleId"" = 1 AND ra.""ActionId"" = a.""Id""
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
