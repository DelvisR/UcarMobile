using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService");

            migrationBuilder.AlterColumn<string>(
                name: "CustomService",
                table: "AppointmentService",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "AppointmentService",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AppointmentService",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppointmentService",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_AppointmentVehicleId",
                table: "AppointmentService",
                column: "AppointmentVehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentService_AppointmentVehicleId",
                table: "AppointmentService");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AppointmentService");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppointmentService");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "AppointmentService",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomService",
                table: "AppointmentService",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService",
                columns: new[] { "AppointmentVehicleId", "ServiceId" });
        }
    }
}
