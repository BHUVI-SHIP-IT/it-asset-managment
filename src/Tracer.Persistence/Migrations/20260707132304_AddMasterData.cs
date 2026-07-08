using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class AddMasterData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Companies",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Companies", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                Error = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxMessages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Permissions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Permissions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Roles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StatusLabels",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                IsDeployable = table.Column<bool>(type: "bit", nullable: false),
                IsPending = table.Column<bool>(type: "bit", nullable: false),
                IsArchived = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StatusLabels", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
                table.ForeignKey(
                    name: "FK_Categories_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Departments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Departments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Departments_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Locations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Locations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Locations_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Manufacturers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Manufacturers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Manufacturers_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Suppliers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Suppliers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Suppliers_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RolePermissions",
            columns: table => new
            {
                RoleId = table.Column<int>(type: "int", nullable: false),
                PermissionId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                table.ForeignKey(
                    name: "FK_RolePermissions_Permissions_PermissionId",
                    column: x => x.PermissionId,
                    principalTable: "Permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RolePermissions_Roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RefreshToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                RefreshTokenExpiryUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Users_Roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AssetModels",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssetModels", x => x.Id);
                table.ForeignKey(
                    name: "FK_AssetModels_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AssetModels_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AssetModels_Manufacturers_ManufacturerId",
                    column: x => x.ManufacturerId,
                    principalTable: "Manufacturers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Assets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                AssetTag = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                SerialNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssetModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StatusLabelId = table.Column<int>(type: "int", nullable: false),
                LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                AssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CheckedOutAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastCheckinAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                PurchaseCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Assets", x => x.Id);
                table.ForeignKey(
                    name: "FK_Assets_AssetModels_AssetModelId",
                    column: x => x.AssetModelId,
                    principalTable: "AssetModels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Assets_Companies_CompanyId",
                    column: x => x.CompanyId,
                    principalTable: "Companies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Assets_Locations_LocationId",
                    column: x => x.LocationId,
                    principalTable: "Locations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Assets_StatusLabels_StatusLabelId",
                    column: x => x.StatusLabelId,
                    principalTable: "StatusLabels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Assets_Users_AssignedUserId",
                    column: x => x.AssignedUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.InsertData(
            table: "Companies",
            columns: new[] { "Id", "Name" },
            values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "Tracer Headquarters" });

        migrationBuilder.InsertData(
            table: "Permissions",
            columns: new[] { "Id", "Description", "Name" },
            values: new object[,]
            {
                { 1, "Allows Assets.View", "Assets.View" },
                { 2, "Allows Assets.Create", "Assets.Create" },
                { 3, "Allows Assets.Edit", "Assets.Edit" },
                { 4, "Allows Assets.Delete", "Assets.Delete" },
                { 5, "Allows Assets.Assign", "Assets.Assign" },
                { 6, "Allows Assets.CheckOut", "Assets.CheckOut" },
                { 7, "Allows Assets.CheckIn", "Assets.CheckIn" },
                { 8, "Allows Assets.Transfer", "Assets.Transfer" },
                { 9, "Allows Assets.Clone", "Assets.Clone" },
                { 10, "Allows Assets.Dispose", "Assets.Dispose" },
                { 11, "Allows Assets.Archive", "Assets.Archive" },
                { 12, "Allows Users.View", "Users.View" },
                { 13, "Allows Users.Create", "Users.Create" },
                { 14, "Allows Users.Edit", "Users.Edit" },
                { 15, "Allows Users.Delete", "Users.Delete" },
                { 16, "Allows Roles.Manage", "Roles.Manage" },
                { 17, "Allows Permissions.Manage", "Permissions.Manage" },
                { 18, "Allows Reports.View", "Reports.View" },
                { 19, "Allows Reports.Export", "Reports.Export" },
                { 20, "Allows Settings.Manage", "Settings.Manage" },
                { 21, "Allows API.Manage", "API.Manage" },
                { 22, "Allows Notifications.Manage", "Notifications.Manage" },
                { 23, "Allows Maintenance.Manage", "Maintenance.Manage" },
                { 24, "Allows AuditLogs.View", "AuditLogs.View" },
                { 25, "Allows Licenses.Manage", "Licenses.Manage" },
                { 26, "Allows Accessories.Manage", "Accessories.Manage" },
                { 27, "Allows Components.Manage", "Components.Manage" },
                { 28, "Allows Consumables.Manage", "Consumables.Manage" }
            });

        migrationBuilder.InsertData(
            table: "Roles",
            columns: new[] { "Id", "Description", "Name" },
            values: new object[,]
            {
                { 1, "Ultimate authority over the Tracer system", "SuperAdmin" },
                { 2, "Technical management of ITAM application", "SystemAdmin" },
                { 3, "Oversight of IT assets and workflows", "ITAdmin" },
                { 4, "Strategic management of asset portfolio", "AssetManager" },
                { 5, "Daily physical management of inventory", "InventoryManager" },
                { 6, "Managing supplier and inbound purchases", "ProcurementOfficer" },
                { 7, "Oversight of departmental assets", "DepartmentManager" },
                { 8, "Managing financial lifecycle of assets", "FinanceOfficer" },
                { 9, "Independent verification of compliance", "Auditor" },
                { 10, "Frontline support managing deployments", "HelpDesk" },
                { 11, "Standard end-user", "Employee" },
                { 12, "Unauthenticated or restricted access", "Guest" }
            });

        migrationBuilder.InsertData(
            table: "StatusLabels",
            columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "IsArchived", "IsDeleted", "IsDeployable", "IsPending", "Name", "RowVersion", "UpdatedAtUtc", "UpdatedBy" },
            values: new object[,]
            {
                { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, false, true, false, "Deployable", new byte[0], null, null },
                { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, false, false, false, "Deployed", new byte[0], null, null },
                { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, true, false, false, false, "Archived", new byte[0], null, null },
                { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, false, false, false, "Broken", new byte[0], null, null },
                { 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, false, false, true, "Pending", new byte[0], null, null }
            });

        migrationBuilder.InsertData(
            table: "RolePermissions",
            columns: new[] { "PermissionId", "RoleId" },
            values: new object[,]
            {
                { 1, 1 },
                { 2, 1 },
                { 3, 1 },
                { 4, 1 },
                { 5, 1 },
                { 6, 1 },
                { 7, 1 },
                { 8, 1 },
                { 9, 1 },
                { 10, 1 },
                { 11, 1 },
                { 12, 1 },
                { 13, 1 },
                { 14, 1 },
                { 15, 1 },
                { 16, 1 },
                { 17, 1 },
                { 18, 1 },
                { 19, 1 },
                { 20, 1 },
                { 21, 1 },
                { 22, 1 },
                { 23, 1 },
                { 24, 1 },
                { 25, 1 },
                { 26, 1 },
                { 27, 1 },
                { 28, 1 },
                { 1, 5 },
                { 2, 5 },
                { 3, 5 },
                { 6, 5 },
                { 7, 5 },
                { 1, 10 },
                { 6, 10 },
                { 7, 10 },
                { 12, 10 },
                { 23, 10 }
            });

        migrationBuilder.InsertData(
            table: "Users",
            columns: new[] { "Id", "CompanyId", "Email", "FullName", "IsActive", "LastLoginAtUtc", "PasswordHash", "RefreshToken", "RefreshTokenExpiryUtc", "RoleId" },
            values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001"), "admin@tracer.io", "System Administrator", true, null, "$2a$11$N9V2V2W41q4.F854hV5/Z.tJjU.n/q.4mO1h3Z/g71.p3z7g91/m6", null, null, 1 });

        migrationBuilder.CreateIndex(
            name: "IX_AssetModels_CategoryId",
            table: "AssetModels",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_AssetModels_CompanyId",
            table: "AssetModels",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_AssetModels_ManufacturerId",
            table: "AssetModels",
            column: "ManufacturerId");

        migrationBuilder.CreateIndex(
            name: "IX_AssetModels_Name",
            table: "AssetModels",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_Assets_AssetModelId",
            table: "Assets",
            column: "AssetModelId");

        migrationBuilder.CreateIndex(
            name: "IX_Assets_AssignedUserId",
            table: "Assets",
            column: "AssignedUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Assets_CompanyId_StatusLabelId",
            table: "Assets",
            columns: new[] { "CompanyId", "StatusLabelId" });

        migrationBuilder.CreateIndex(
            name: "IX_Assets_LocationId",
            table: "Assets",
            column: "LocationId");

        migrationBuilder.CreateIndex(
            name: "IX_Assets_StatusLabelId",
            table: "Assets",
            column: "StatusLabelId");

        migrationBuilder.CreateIndex(
            name: "UX_Assets_CompanyId_AssetTag",
            table: "Assets",
            columns: new[] { "CompanyId", "AssetTag" },
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_Categories_CompanyId",
            table: "Categories",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Categories_Name",
            table: "Categories",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "UX_Companies_Name",
            table: "Companies",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Departments_CompanyId",
            table: "Departments",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Departments_Name",
            table: "Departments",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_Locations_CompanyId",
            table: "Locations",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Locations_Name",
            table: "Locations",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_Manufacturers_CompanyId",
            table: "Manufacturers",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Manufacturers_Name",
            table: "Manufacturers",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_ProcessedOnUtc",
            table: "OutboxMessages",
            column: "ProcessedOnUtc",
            filter: "[ProcessedOnUtc] IS NULL");

        migrationBuilder.CreateIndex(
            name: "UX_Permissions_Name",
            table: "Permissions",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_PermissionId",
            table: "RolePermissions",
            column: "PermissionId");

        migrationBuilder.CreateIndex(
            name: "UX_Roles_Name",
            table: "Roles",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_StatusLabels_Name",
            table: "StatusLabels",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_Suppliers_CompanyId",
            table: "Suppliers",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Suppliers_Name",
            table: "Suppliers",
            column: "Name",
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_Users_CompanyId",
            table: "Users",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_RoleId",
            table: "Users",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "UX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Assets");

        migrationBuilder.DropTable(
            name: "Departments");

        migrationBuilder.DropTable(
            name: "OutboxMessages");

        migrationBuilder.DropTable(
            name: "RolePermissions");

        migrationBuilder.DropTable(
            name: "Suppliers");

        migrationBuilder.DropTable(
            name: "AssetModels");

        migrationBuilder.DropTable(
            name: "Locations");

        migrationBuilder.DropTable(
            name: "StatusLabels");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Permissions");

        migrationBuilder.DropTable(
            name: "Categories");

        migrationBuilder.DropTable(
            name: "Manufacturers");

        migrationBuilder.DropTable(
            name: "Roles");

        migrationBuilder.DropTable(
            name: "Companies");
    }
}
