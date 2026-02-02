using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RectifyDeviceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Device_UserId_Token",
                table: "Device");

            migrationBuilder.CreateIndex(
                name: "IX_Device_UserId_Token_Platform",
                table: "Device",
                columns: new[] { "UserId", "Token", "Platform" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Device_UserId_Token_Platform",
                table: "Device");

            migrationBuilder.CreateIndex(
                name: "IX_Device_UserId_Token",
                table: "Device",
                columns: new[] { "UserId", "Token" },
                unique: true);
        }
    }
}
