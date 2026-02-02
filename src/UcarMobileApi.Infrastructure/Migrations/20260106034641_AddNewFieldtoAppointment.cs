using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldtoAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Appointment",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Appointment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarrantyMonths",
                table: "Appointment",
                type: "integer",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Value", "ValueType", "Description"],
                values: new object[,]
                {
                    {
                        "WARRANTY_MONTHS", "12", "int",
                        "Warranty period in months for the completed repair or service"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "WarrantyMonths",
                table: "Appointment");
        }
    }
}
