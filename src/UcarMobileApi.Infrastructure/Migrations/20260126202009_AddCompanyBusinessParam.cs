using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyBusinessParam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "BusinessParameter",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "BusinessParameter",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "DEFAULT_CLIENT_ROLES", columns: ["Category", "Order"],
                values: ["Client & Booking Management", 110]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "BOOKING_CALENDAR_DAYS_AHEAD", columns: ["Category", "Order"],
                values: ["Client & Booking Management", 120]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "CANCELLATION_WINDOW_HOURS", columns: ["Category", "Order"],
                values: ["Client & Booking Management", 130]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "APPROVAL_WINDOW_HOURS", columns: ["Category", "Order"],
                values: ["Client & Booking Management", 140]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "DIAGNOSTIC_WINDOW_MINUTES", columns: ["Category", "Order"],
                values: ["Service Workflow Timing", 210]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "REPAIR_BUFFER_HOURS", columns: ["Category", "Order"],
                values: ["Service Workflow Timing", 220]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "LABOR_DISCOUNT_FACTOR", columns: ["Category", "Order"],
                values: ["Pricing & Discounts", 310]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "PART_DISCOUNT_FACTOR", columns: ["Category", "Order"],
                values: ["Pricing & Discounts", 320]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "REVIEW_DISCOUNT_AMOUNT", columns: ["Category", "Order"],
                values: ["Pricing & Discounts", 330]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "WARRANTY_MONTHS", columns: ["Category", "Order"],
                values: ["Warranty & Service Coverage", 410]);

            migrationBuilder.UpdateData(table: "BusinessParameter", keyColumn: "Key",
                keyValue: "WARRANTY_MILES", columns: ["Category", "Order"],
                values: ["Warranty & Service Coverage", 420]);

            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Category", "Value", "ValueType", "Description", "Order"],
                values: new object[,]
                {
                    {
                        "COMPANY_NAME", "Company Information", "Security Ride, LLC dba UcarMobile", "string",
                        "Registered legal name of the company", 10
                    },
                    {
                        "COMPANY_HEADQUARTERS_ADDRESS", "Company Information", "3400 14th Street Plano, TX 75074 (HQ address)", "string",
                        "Physical address of the company’s headquarters", 20
                    },
                    {
                        "COMPANY_CONTACT_PHONE", "Company Information", "(972) 972-9197", "phone",
                        "Primary phone number to contact the company", 30
                    },
                    {
                        "COMPANY_CONTACT_EMAIL", "Company Information", "contact@ucarmobile.com", "email",
                        "Primary email address to contact the company", 40
                    },
                    {
                        "COMPANY_WEBSITE_URL", "Company Information", "www.ucarmobile.com", "web",
                        "Official website URL of the company", 50
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "BusinessParameter");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "BusinessParameter");
        }
    }
}
