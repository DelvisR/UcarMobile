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
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LangKey = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true, defaultValue: "en"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AuthProviderId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ActionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
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
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
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
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_Name",
                table: "Action",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessParameter_Key",
                table: "BusinessParameter",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Device_UserId_Token",
                table: "Device",
                columns: new[] { "UserId", "Token" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Phone",
                table: "Lead",
                column: "Phone");

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
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_ClientId",
                table: "Vehicle",
                column: "ClientId");

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
                    "UserId", "RoleId", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "IsDeleted"
                ],
                values: [2, 1, "system", DateTime.UtcNow, "system", DateTime.UtcNow, false]
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessParameter");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Lead");

            migrationBuilder.DropTable(
                name: "RoleAction");

            migrationBuilder.DropTable(
                name: "Technician");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "UserAccount");
        }
    }
}
