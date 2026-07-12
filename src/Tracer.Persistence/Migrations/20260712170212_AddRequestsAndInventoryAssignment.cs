using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tracer.Persistence.Migrations;

    /// <inheritdoc />
    public partial class AddRequestsAndInventoryAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Depreciations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "StraightLine");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtUtc",
                table: "Consumables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedUserId",
                table: "Consumables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReorderThreshold",
                table: "Consumables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtUtc",
                table: "Components",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedUserId",
                table: "Components",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompatibleAssetModelId",
                table: "Components",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAtUtc",
                table: "Accessories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedUserId",
                table: "Accessories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolvedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_InventoryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Users_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 81, "Allows Requests.Create", "Requests.Create" },
                    { 82, "Allows Requests.ViewOwn", "Requests.ViewOwn" },
                    { 83, "Allows Requests.ViewAll", "Requests.ViewAll" },
                    { 84, "Allows Requests.Approve", "Requests.Approve" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    // SuperAdmin
                    { 81, 1 },
                    { 82, 1 },
                    { 83, 1 },
                    { 84, 1 },
                    // SystemAdmin, ITAdmin, AssetManager, InventoryManager, HelpDesk — approve queue
                    { 83, 2 }, { 84, 2 },
                    { 83, 3 }, { 84, 3 },
                    { 83, 4 }, { 84, 4 },
                    { 83, 5 }, { 84, 5 },
                    { 83, 10 }, { 84, 10 },
                    // Employee — create + view own
                    { 81, 11 },
                    { 82, 11 },
                    // SalesRep (also Employee role id 11 for one user; SalesRep is role 11... wait SalesRep is RoleId 11 Employee. Chris is also Employee role 11.
                    // DepartmentManager can create requests too
                    { 81, 7 },
                    { 82, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consumables_AssignedUserId",
                table: "Consumables",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_AssignedUserId",
                table: "Components",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_CompatibleAssetModelId",
                table: "Components",
                column: "CompatibleAssetModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Accessories_AssignedUserId",
                table: "Accessories",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_CompanyId_Status",
                table: "InventoryRequests",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_RequestedByUserId",
                table: "InventoryRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_ResolvedByUserId",
                table: "InventoryRequests",
                column: "ResolvedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accessories_Users_AssignedUserId",
                table: "Accessories",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_AssetModels_CompatibleAssetModelId",
                table: "Components",
                column: "CompatibleAssetModelId",
                principalTable: "AssetModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Users_AssignedUserId",
                table: "Components",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consumables_Users_AssignedUserId",
                table: "Consumables",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accessories_Users_AssignedUserId",
                table: "Accessories");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_AssetModels_CompatibleAssetModelId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_Users_AssignedUserId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Consumables_Users_AssignedUserId",
                table: "Consumables");

            migrationBuilder.DropTable(
                name: "InventoryRequests");

            migrationBuilder.DropIndex(
                name: "IX_Consumables_AssignedUserId",
                table: "Consumables");

            migrationBuilder.DropIndex(
                name: "IX_Components_AssignedUserId",
                table: "Components");

            migrationBuilder.DropIndex(
                name: "IX_Components_CompatibleAssetModelId",
                table: "Components");

            migrationBuilder.DropIndex(
                name: "IX_Accessories_AssignedUserId",
                table: "Accessories");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 81, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 82, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 83, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 84, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[,]
                {
                    { 83, 2 }, { 84, 2 },
                    { 83, 3 }, { 84, 3 },
                    { 83, 4 }, { 84, 4 },
                    { 83, 5 }, { 84, 5 },
                    { 83, 10 }, { 84, 10 },
                    { 81, 11 }, { 82, 11 },
                    { 81, 7 }, { 82, 7 }
                });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Depreciations");

            migrationBuilder.DropColumn(
                name: "AssignedAtUtc",
                table: "Consumables");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Consumables");

            migrationBuilder.DropColumn(
                name: "ReorderThreshold",
                table: "Consumables");

            migrationBuilder.DropColumn(
                name: "AssignedAtUtc",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "CompatibleAssetModelId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "AssignedAtUtc",
                table: "Accessories");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Accessories");
        }
    }
