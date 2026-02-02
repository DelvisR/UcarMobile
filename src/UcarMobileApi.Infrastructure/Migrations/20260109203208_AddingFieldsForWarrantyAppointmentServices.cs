using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingFieldsForWarrantyAppointmentServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWarrantyApplicable",
                table: "ServiceType",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomerProvidedPart",
                table: "AppointmentPart",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Value", "ValueType", "Description"],
                values: new object[,]
                {
                    {
                        "APPROVAL_WINDOW_HOURS", "48", "int",
                        "Defines the maximum number of hours a client has to approve a scheduled repair appointment before the technician assignment automatically expires"
                    }
                });

            migrationBuilder.Sql(@"UPDATE ""ServiceType"" SET ""IsWarrantyApplicable"" = TRUE WHERE ""Id"" IN (1,2,9,14,15);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWarrantyApplicable",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "IsCustomerProvidedPart",
                table: "AppointmentPart");
        }
    }
}
