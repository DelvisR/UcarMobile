using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageStoredFileId",
                table: "UserAccount",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OdometerKm",
                table: "ClientVehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_ImageStoredFileId",
                table: "UserAccount",
                column: "ImageStoredFileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccount_StoredFile_ImageStoredFileId",
                table: "UserAccount",
                column: "ImageStoredFileId",
                principalTable: "StoredFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccount_StoredFile_ImageStoredFileId",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_UserAccount_ImageStoredFileId",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "ImageStoredFileId",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "OdometerKm",
                table: "ClientVehicle");
        }
    }
}
