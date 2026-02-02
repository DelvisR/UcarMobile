using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingAppointmentDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Appointment",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AppointmentDiscount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentId = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Source = table.Column<byte>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentDiscount_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDiscount_AppointmentId_Category",
                table: "AppointmentDiscount",
                columns: new[] { "AppointmentId", "Category" },
                unique: true);

            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns: ["Key", "Value", "ValueType", "Description"],
                values: new object[,]
                {
                    {
                        "REVIEW_DISCOUNT_AMOUNT", "5", "decimal",
                        "Fixed discount amount applied when the Client submits a review on the website"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentDiscount");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Appointment");
        }
    }
}
