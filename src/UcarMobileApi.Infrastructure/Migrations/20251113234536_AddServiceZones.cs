using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceZone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    BaseAddress = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    RadiusMiles = table.Column<double>(type: "double precision", nullable: false, defaultValue: 25.0),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lng = table.Column<double>(type: "double precision", nullable: false),
                    ZipCodes = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceZone", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceZone_BaseAddress",
                table: "ServiceZone",
                column: "BaseAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceZone_IsActive",
                table: "ServiceZone",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceZone_ZipCodes",
                table: "ServiceZone",
                column: "ZipCodes")
                .Annotation("Npgsql:IndexMethod", "gin");

            // Initial Seed
            migrationBuilder.InsertData(
                table: "ServiceZones",
                columns: ["Id", "Name", "BaseAddress", "RadiusMiles", "Lat", "Lng", "ZipCodes", "IsActive", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "IsDeleted"
                ],
                values: new object[]
                {
                    1,
                    "UcarMobile",
                    "3400 14th Street Plano, TX",
                    25.0,
                    33.0146527,
                    -96.6702438,
                    "[\"75074\"]", // JSON string for jsonb field
                    true,
                    "system",
                    DateTime.UtcNow,
                    "system",
                    DateTime.UtcNow,
                    false
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceZone");
        }
    }
}
