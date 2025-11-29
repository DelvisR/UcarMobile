using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UcarMobileApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Resource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "RESOURCE_DEFAULT"),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessParameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ValueType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessParameter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lead",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lead", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceType", x => x.Id);
                });

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
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceZone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Bucket = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LangKey = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true, defaultValue: "en"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AuthProviderId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Make = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ActionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAction_Action_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Action",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleAction_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ServiceTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCategory_ServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ProviderPaymentCustomerId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Client_UserAccount_Id",
                        column: x => x.Id,
                        principalTable: "UserAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<byte>(type: "smallint", nullable: false),
                    EndpointArn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_UserAccount_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Technician",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsFreelance = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technician", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Technician_UserAccount_Id",
                        column: x => x.Id,
                        principalTable: "UserAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_UserAccount_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ServiceCategoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_ServiceCategory_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ScheduledStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ServiceAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lng = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentStatus = table.Column<byte>(type: "smallint", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientVehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    VIN = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    LicensePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Submodel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EngineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VehicleType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BodyType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientVehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientVehicle_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientVehicle_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ProviderPaymentMethodId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Brand = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Last4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    ExpMonth = table.Column<int>(type: "integer", nullable: false),
                    ExpYear = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethod_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianServiceZone",
                columns: table => new
                {
                    TechnicianId = table.Column<int>(type: "integer", nullable: false),
                    ServiceZoneId = table.Column<int>(type: "integer", nullable: false),
                    IsPrimaryZone = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianServiceZone", x => new { x.TechnicianId, x.ServiceZoneId });
                    table.ForeignKey(
                        name: "FK_TechnicianServiceZone_ServiceZone_ServiceZoneId",
                        column: x => x.ServiceZoneId,
                        principalTable: "ServiceZone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicianServiceZone_Technician_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technician",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSpeciality",
                columns: table => new
                {
                    TechnicianId = table.Column<int>(type: "integer", nullable: false),
                    ServiceCategoryId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<byte>(type: "smallint", nullable: false),
                    IsCertified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSpeciality", x => new { x.TechnicianId, x.ServiceCategoryId });
                    table.ForeignKey(
                        name: "FK_TechnicianSpeciality_ServiceCategory_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicianSpeciality_Technician_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technician",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estimate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    LaborMaxCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LaborMinCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PartMaxCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PartMinCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estimate_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estimate_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentNote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Source = table.Column<byte>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentNote_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentVehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentId = table.Column<int>(type: "integer", nullable: false),
                    TechnicianId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentVehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentVehicle_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentVehicle_ClientVehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "ClientVehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentVehicle_Technician_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technician",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "integer", nullable: false),
                    ProviderPaymentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    AmountCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "usd"),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ClientSecret = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ErrorCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AppointmentId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payment_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentId = table.Column<int>(type: "integer", nullable: false),
                    StoredFileId = table.Column<int>(type: "integer", nullable: false),
                    AppointmentNoteId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentDocument_AppointmentNote_AppointmentNoteId",
                        column: x => x.AppointmentNoteId,
                        principalTable: "AppointmentNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentDocument_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentDocument_StoredFile_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentPart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentVehicleId = table.Column<int>(type: "integer", nullable: false),
                    PartName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PartNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentPart_AppointmentVehicle_AppointmentVehicleId",
                        column: x => x.AppointmentVehicleId,
                        principalTable: "AppointmentVehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentVehicleId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentService_AppointmentVehicle_AppointmentVehicleId",
                        column: x => x.AppointmentVehicleId,
                        principalTable: "AppointmentVehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRefund",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<int>(type: "integer", nullable: false),
                    ProviderRefundId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AmountCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "usd"),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "System"),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRefund", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRefund_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_Name",
                table: "Action",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ClientId",
                table: "Appointment",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_Lat_Lng",
                table: "Appointment",
                columns: new[] { "Lat", "Lng" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PaymentStatus",
                table: "Appointment",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ScheduledStart",
                table: "Appointment",
                column: "ScheduledStart");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_Status",
                table: "Appointment",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDocument_AppointmentId",
                table: "AppointmentDocument",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDocument_AppointmentNoteId",
                table: "AppointmentDocument",
                column: "AppointmentNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDocument_StoredFileId",
                table: "AppointmentDocument",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentNote_AppointmentId",
                table: "AppointmentNote",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentNote_CreatedDate",
                table: "AppointmentNote",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentNote_Source",
                table: "AppointmentNote",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPart_AppointmentVehicleId",
                table: "AppointmentPart",
                column: "AppointmentVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPart_PartNumber",
                table: "AppointmentPart",
                column: "PartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_AppointmentVehicleId",
                table: "AppointmentService",
                column: "AppointmentVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_ServiceId",
                table: "AppointmentService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentVehicle_AppointmentId",
                table: "AppointmentVehicle",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentVehicle_AppointmentId_VehicleId_TechnicianId",
                table: "AppointmentVehicle",
                columns: new[] { "AppointmentId", "VehicleId", "TechnicianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentVehicle_TechnicianId",
                table: "AppointmentVehicle",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentVehicle_VehicleId",
                table: "AppointmentVehicle",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessParameter_Key",
                table: "BusinessParameter",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_ClientId",
                table: "ClientVehicle",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_ClientId_VehicleId",
                table: "ClientVehicle",
                columns: new[] { "ClientId", "VehicleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_LicensePlate",
                table: "ClientVehicle",
                column: "LicensePlate");

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_VehicleId",
                table: "ClientVehicle",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientVehicle_VIN",
                table: "ClientVehicle",
                column: "VIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Device_UserId_Token",
                table: "Device",
                columns: new[] { "UserId", "Token" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estimate_ServiceId",
                table: "Estimate",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimate_ServiceId_VehicleId",
                table: "Estimate",
                columns: new[] { "ServiceId", "VehicleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estimate_VehicleId",
                table: "Estimate",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Phone",
                table: "Lead",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AppointmentId",
                table: "Payment",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ClientId",
                table: "Payment",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentMethodId",
                table: "Payment",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ProviderPaymentId",
                table: "Payment",
                column: "ProviderPaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_ClientId_ProviderPaymentMethodId",
                table: "PaymentMethod",
                columns: new[] { "ClientId", "ProviderPaymentMethodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRefund_PaymentId",
                table: "PaymentRefund",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRefund_ProviderRefundId",
                table: "PaymentRefund",
                column: "ProviderRefundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleAction_ActionId",
                table: "RoleAction",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAction_RoleId",
                table: "RoleAction",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceCategoryId",
                table: "Service",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceCategoryId_Name",
                table: "Service",
                columns: new[] { "ServiceCategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategory_ServiceTypeId",
                table: "ServiceCategory",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategory_ServiceTypeId_Name",
                table: "ServiceCategory",
                columns: new[] { "ServiceTypeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceType_Title",
                table: "ServiceType",
                column: "Title",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_StoredFile_FileName",
                table: "StoredFile",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFile_Key",
                table: "StoredFile",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technician_IsFreelance",
                table: "Technician",
                column: "IsFreelance");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServiceZone_ServiceZoneId",
                table: "TechnicianServiceZone",
                column: "ServiceZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSpeciality_ServiceCategoryId",
                table: "TechnicianSpeciality",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_AuthProviderId",
                table: "UserAccount",
                column: "AuthProviderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_Email",
                table: "UserAccount",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Make",
                table: "Vehicle",
                column: "Make");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Make_Model_Year",
                table: "Vehicle",
                columns: new[] { "Make", "Model", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Year",
                table: "Vehicle",
                column: "Year");

            AddSeedData(migrationBuilder);
        }

        private static void AddSeedData(MigrationBuilder migrationBuilder)
        {
            // seed data

            migrationBuilder.InsertData(
                table: "Role",
                columns:
                [
                    "Id", "Name", "Description", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate",
                    "IsDeleted"
                ],
                values: new object[,]
                {
                    {
                        1, "SuperAdmin",
                        "Has full control over the platform. Can manage all users (add, modify, delete, deactivate, assign roles), supervise analytics, reports, audit logs, system configurations, and can add other admins.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        2, "Admin",
                        "Almost all capabilities of SuperAdmin except adding new admins. Manages users, schedules, jobs, service zones, payments, etc.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        3, "Technician",
                        "Base role for all technicians. Common actions: view appointments, parts, notes, notifications, use mobile app.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        4, "TechnicianInHouse",
                        "Fixed-schedule contractors assigned jobs by administration. Can request days off. Full mobile app access to manage appointments, parts, notes, etc. Paid weekly/monthly.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        5, "TechnicianFreelancer",
                        "Contractors with flexible availability, accept/reject jobs. Manage workflow in mobile app. Paid based on commissions.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        6, "Client",
                        "Schedules, cancels or reschedules appointments, receives updates, notifications, invoices, can view service history, rate technicians, communicate with them or the call center, manage profile and registered vehicles.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        7, "FleetOwner",
                        "Specialized client managing multiple vehicles. Can schedule appointments for several vehicles, add/manage users/managers in the account, receives centralized billing.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        8, "FleetManager",
                        "Delegated by Fleet Owner. Can schedule, reschedule, cancel appointments, add notes or updates to appointments on behalf of Fleet Owner.",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    }
                });

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
                        "ACTION_CREATE_USER", "Allows creating new users", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_USER", "Allows editing existing users", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_CREATE_CLIENT", "Allows creating new clients", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_CLIENT", "Allows editing existing clients", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_CREATE_ROLE", "Allows creating new roles", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_ROLE", "Allows editing existing roles", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_DELETE_ROLE", "Allows deleting roles", "RESOURCE_DEFAULT", "System", DateTime.UtcNow,
                        "System",
                        DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_BUSINESS_PARAMETER", "Allows edit existing business parameter", "RESOURCE_DEFAULT",
                        "System", DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_LEAD", "Allows editing existing leads", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow,
                        "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_DELETE_LEAD", "Allows deleting leads", "RESOURCE_DEFAULT", "System", DateTime.UtcNow,
                        "System",
                        DateTime.UtcNow, false
                    },
                    {
                        "ACTION_UPLOAD_FILE", "Allows uploads file to AWS S3", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_FILE", "Allows view file from AWS S3", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_DELETE_FILE", "Allows delete file from AWS S3", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_REGISTER_DEVICE", "Allows register devices for push notifications", "RESOURCE_DEFAULT",
                        "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_SEND_NOTIFICATION", "Allows send notifications", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_PAYMENT_METHODS", "Allows view payments methods", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_MANAGE_PAYMENTS", "Allows manage payments", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_CREATE_PAYMENT", "Allows create payments", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_REFUND_PAYMENT", "Allows refund payments", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_CREATE_TECHNICIAN", "Allows creating new technician", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_TECHNICIAN", "Allows editing existing technician", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_SERVICE_ZONES", "Allows view service zones", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_CREATE_SERVICE_ZONE", "Allows creating service zones", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_EDIT_SERVICE_ZONE", "Allows editing service zones", "RESOURCE_DEFAULT", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    }
                });

            // Insert new actions with category "RESOURCE_MAIN_MENU"
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
                        "ACTION_MANAGE_ROLE_RESOURCE_AND_ACTIONS", "Admin menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_DASHBOARD", "Dashboard menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_ESTIMATE", "Estimate menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_USERS", "Users menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_CLIENTS", "Clients menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_ROLES", "Roles menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_BUSINESS_PARAMETER", "Bussiness parameters menu action",
                        "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_LEADS", "Leads menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_TECHNICIANS", "Technician menu action", "RESOURCE_MAIN_MENU", "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    },
                    {
                        "ACTION_VIEW_MAIN_MENU_SERVICE_ZONES", "Service Zones menu action", "RESOURCE_MAIN_MENU",
                        "System",
                        DateTime.UtcNow, "System", DateTime.UtcNow, false
                    }
                });

            // Insert default values
            migrationBuilder.InsertData(
                table: "BusinessParameter",
                columns:
                [
                    "Key", "Value", "ValueType", "Description", "CreatedBy", "CreatedDate", "LastModifiedBy",
                    "LastModifiedDate", "IsDeleted"
                ],
                values: new object[,]
                {
                    {
                        "DefaultClientRoles", "6", "string", "Default Role for new Client", "system", DateTime.UtcNow,
                        "system", DateTime.UtcNow, false
                    }
                });

            migrationBuilder.InsertData(
                table: "UserAccount",
                columns:
                [
                    "Id", "Email", "Phone", "FirstName", "LastName", "LangKey", "IsActive", "AuthProviderId",
                    "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "IsDeleted"
                ],
                values: new object[,]
                {
                    {
                        1, "system@ucarmobile.com", "0000000000", "System", "Root", "en", true,
                        Guid.NewGuid().ToString(),
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    },
                    {
                        2, "admin@ucarmobile.com", "1111111111", "Super", "Admin", "en", true,
                        "143884b8-b0a1-702e-1a8f-547574f2ed9c",
                        "system", DateTime.UtcNow, "system", DateTime.UtcNow, false
                    }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns:
                [
                    "UserId", "RoleId", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate"
                ],
                values: [2, 1, "system", DateTime.UtcNow, "system", DateTime.UtcNow]
            );

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

            // assign some actions to Client rol
            migrationBuilder.Sql(@"
                INSERT INTO ""RoleAction"" (""RoleId"", ""ActionId"", ""CreatedBy"", ""CreatedDate"", ""LastModifiedBy"", ""LastModifiedDate"", ""IsDeleted"")
                SELECT 6, a.""Id"", 'system', NOW(), 'system', NOW(), false
                FROM ""Action"" a
                WHERE a.""Id"" IN (10, 11)
                  AND NOT EXISTS (
                      SELECT 1 FROM ""RoleAction"" ra
                      WHERE ra.""RoleId"" = 6 AND ra.""ActionId"" = a.""Id""
                  );
            ");

            // ServiceZone Initial Data
            migrationBuilder.InsertData(
                table: "ServiceZone",
                columns:
                [
                    "Id", "Name", "BaseAddress", "RadiusMiles", "Lat", "Lng", "ZipCodes", "IsActive", "CreatedBy",
                    "CreatedDate", "LastModifiedBy", "LastModifiedDate", "IsDeleted"
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

            // Initial Data for Service Types
            migrationBuilder.InsertData(
                table: "ServiceType",
                columns: ["Id", "Title"],
                values: new object[,]
                {
                    { 1, "Mechanical Repair Estimate" },

                });

            // Adding some initial categories
            migrationBuilder.InsertData(
                table: "ServiceCategory",
                columns: ["Name", "ServiceTypeId"],
                values: new object[,]
                {
                    { "Body, Glass & Interior", 1 },
                    { "Brakes", 1 },
                    { "Diagnosis & Testing", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentDocument");

            migrationBuilder.DropTable(
                name: "AppointmentPart");

            migrationBuilder.DropTable(
                name: "AppointmentService");

            migrationBuilder.DropTable(
                name: "BusinessParameter");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Estimate");

            migrationBuilder.DropTable(
                name: "Lead");

            migrationBuilder.DropTable(
                name: "PaymentRefund");

            migrationBuilder.DropTable(
                name: "RoleAction");

            migrationBuilder.DropTable(
                name: "TechnicianServiceZone");

            migrationBuilder.DropTable(
                name: "TechnicianSpeciality");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "AppointmentNote");

            migrationBuilder.DropTable(
                name: "StoredFile");

            migrationBuilder.DropTable(
                name: "AppointmentVehicle");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "ServiceZone");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "ClientVehicle");

            migrationBuilder.DropTable(
                name: "Technician");

            migrationBuilder.DropTable(
                name: "ServiceCategory");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "UserAccount");
        }
    }
}
