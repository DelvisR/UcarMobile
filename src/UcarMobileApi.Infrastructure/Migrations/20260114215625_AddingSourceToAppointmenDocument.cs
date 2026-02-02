using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSourceToAppointmenDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Source",
                table: "AppointmentNote",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)1,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<byte>(
                name: "Source",
                table: "AppointmentDocument",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "AppointmentDocument");

            migrationBuilder.AlterColumn<byte>(
                name: "Source",
                table: "AppointmentNote",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldDefaultValue: (byte)1);
        }
    }
}
