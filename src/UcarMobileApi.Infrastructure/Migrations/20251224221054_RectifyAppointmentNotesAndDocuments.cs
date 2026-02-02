using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RectifyAppointmentNotesAndDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDocument_AppointmentNote_AppointmentNoteId",
                table: "AppointmentDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDocument_Appointment_AppointmentId",
                table: "AppointmentDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDocument_StoredFile_StoredFileId",
                table: "AppointmentDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentNote_Appointment_AppointmentId",
                table: "AppointmentNote");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDocument_AppointmentNoteId",
                table: "AppointmentDocument");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDocument_StoredFileId",
                table: "AppointmentDocument");

            migrationBuilder.DropColumn(
                name: "AppointmentNoteId",
                table: "AppointmentDocument");

            migrationBuilder.CreateTable(
                name: "AppointmentNoteDocument",
                columns: table => new
                {
                    AppointmentNoteId = table.Column<int>(type: "integer", nullable: false),
                    StoredFileId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentNoteDocument", x => new { x.AppointmentNoteId, x.StoredFileId });
                    table.ForeignKey(
                        name: "FK_AppointmentNoteDocument_AppointmentNote_AppointmentNoteId",
                        column: x => x.AppointmentNoteId,
                        principalTable: "AppointmentNote",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppointmentNoteDocument_StoredFile_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDocument_StoredFileId",
                table: "AppointmentDocument",
                column: "StoredFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentNoteDocument_StoredFileId",
                table: "AppointmentNoteDocument",
                column: "StoredFileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDocument_Appointment_AppointmentId",
                table: "AppointmentDocument",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDocument_StoredFile_StoredFileId",
                table: "AppointmentDocument",
                column: "StoredFileId",
                principalTable: "StoredFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentNote_Appointment_AppointmentId",
                table: "AppointmentNote",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDocument_Appointment_AppointmentId",
                table: "AppointmentDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDocument_StoredFile_StoredFileId",
                table: "AppointmentDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentNote_Appointment_AppointmentId",
                table: "AppointmentNote");

            migrationBuilder.DropTable(
                name: "AppointmentNoteDocument");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDocument_StoredFileId",
                table: "AppointmentDocument");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentNoteId",
                table: "AppointmentDocument",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDocument_AppointmentNoteId",
                table: "AppointmentDocument",
                column: "AppointmentNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDocument_StoredFileId",
                table: "AppointmentDocument",
                column: "StoredFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDocument_AppointmentNote_AppointmentNoteId",
                table: "AppointmentDocument",
                column: "AppointmentNoteId",
                principalTable: "AppointmentNote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDocument_Appointment_AppointmentId",
                table: "AppointmentDocument",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDocument_StoredFile_StoredFileId",
                table: "AppointmentDocument",
                column: "StoredFileId",
                principalTable: "StoredFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentNote_Appointment_AppointmentId",
                table: "AppointmentNote",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
