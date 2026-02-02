using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewBusinessParam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Value", "ValueType", "Description"],
                values: new object[,]
                {
                    {
                        "CANCELLATION_WINDOW_HOURS", "4", "int",
                        "Number of hours before the scheduled service during which cancellation is allowed"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
