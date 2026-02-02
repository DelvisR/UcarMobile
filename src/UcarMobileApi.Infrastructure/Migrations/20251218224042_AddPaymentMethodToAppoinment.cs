using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodToAppoinment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "Appointment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PaymentMethodId",
                table: "Appointment",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_PaymentMethod_PaymentMethodId",
                table: "Appointment",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_PaymentMethod_PaymentMethodId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_PaymentMethodId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Appointment");
        }
    }
}
