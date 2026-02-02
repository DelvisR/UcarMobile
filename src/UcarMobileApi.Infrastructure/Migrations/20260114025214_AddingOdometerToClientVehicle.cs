using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingOdometerToClientVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OdometerKm",
                table: "ClientVehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // assign client actions to Client rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"")
                SELECT 3, a.""Id"" FROM ""Action"" a
                WHERE a.""Name"" IN ('ACTION_EDIT_CLIENT_VEHICLE')
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 3 AND ra.""ActionId"" = a.""Id""
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdometerKm",
                table: "ClientVehicle");
        }
    }
}
