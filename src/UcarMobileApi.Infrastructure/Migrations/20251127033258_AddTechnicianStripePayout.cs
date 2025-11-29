using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianStripePayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderAccountId",
                table: "Technician",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderDisplayName",
                table: "Technician",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProviderPaymentsEnabled",
                table: "Technician",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Payout",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TechnicianId = table.Column<int>(type: "integer", nullable: false),
                    AmountCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "usd"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderPayoutId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProviderDestination = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payout_Technician_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technician",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payout_ProviderPayoutId",
                table: "Payout",
                column: "ProviderPayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Payout_Status",
                table: "Payout",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payout_TechnicianId",
                table: "Payout",
                column: "TechnicianId");

            // seed data

            // Insert existing actions with default category "RESOURCE_DEFAULT"
            migrationBuilder.InsertData(
                table: "Action",
                columns:
                [
                    "Name", "Description", "Resource", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate",
                    "IsDeleted"
                ],
                values: new object[,]
                {
                    {
                        "ACTION_MANAGE_PAYOUTS", "Allows manage payouts", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_CREATE_PAYOUTS", "Allows create payouts", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                });

            // assign all acction to SuperAdmin rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"", ""CreatedBy"", ""CreatedDate"", ""LastModifiedBy"", ""LastModifiedDate"", ""IsDeleted"")
                SELECT 1, a.""Id"", 'system', NOW(), 'system', NOW(), false
                FROM ""Action"" a
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""RoleAction"" ra 
                    WHERE ra.""RoleId"" = 1 AND ra.""ActionId"" = a.""Id""
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payout");

            migrationBuilder.DropColumn(
                name: "ProviderAccountId",
                table: "Technician");

            migrationBuilder.DropColumn(
                name: "ProviderDisplayName",
                table: "Technician");

            migrationBuilder.DropColumn(
                name: "ProviderPaymentsEnabled",
                table: "Technician");
        }
    }
}
