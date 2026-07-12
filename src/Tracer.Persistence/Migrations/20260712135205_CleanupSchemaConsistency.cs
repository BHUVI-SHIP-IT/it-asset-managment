using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class CleanupSchemaConsistency : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Assets is temporal — pause versioning, merge shadow FK, drop column from current + history.
        // Idempotent: safe if a previous attempt already turned versioning off.
        migrationBuilder.Sql("""
            IF EXISTS (
                SELECT 1 FROM sys.tables
                WHERE object_id = OBJECT_ID(N'dbo.Assets') AND temporal_type = 2)
            BEGIN
                ALTER TABLE [Assets] SET (SYSTEM_VERSIONING = OFF);
            END

            IF COL_LENGTH(N'dbo.Assets', N'DepreciationId1') IS NOT NULL
            BEGIN
                UPDATE [Assets]
                SET [DepreciationId] = [DepreciationId1]
                WHERE [DepreciationId] IS NULL AND [DepreciationId1] IS NOT NULL;
            END
            """);

        migrationBuilder.Sql("""
            IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Assets_Depreciations_DepreciationId1')
                ALTER TABLE [Assets] DROP CONSTRAINT [FK_Assets_Depreciations_DepreciationId1];

            IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Assets_DepreciationId1' AND object_id = OBJECT_ID(N'dbo.Assets'))
                DROP INDEX [IX_Assets_DepreciationId1] ON [Assets];

            IF COL_LENGTH(N'dbo.Assets', N'DepreciationId1') IS NOT NULL
                ALTER TABLE [Assets] DROP COLUMN [DepreciationId1];

            IF COL_LENGTH(N'dbo.AssetsHistory', N'DepreciationId1') IS NOT NULL
                ALTER TABLE [AssetsHistory] DROP COLUMN [DepreciationId1];

            IF EXISTS (
                SELECT 1 FROM sys.tables
                WHERE object_id = OBJECT_ID(N'dbo.Assets') AND temporal_type = 0)
               AND OBJECT_ID(N'dbo.AssetsHistory') IS NOT NULL
            BEGIN
                ALTER TABLE [Assets] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[AssetsHistory]));
            END
            """);

        // EF-generated DropForeignKey/DropIndex/DropColumn for DepreciationId1 are replaced by SQL above
        // so they are skipped here when already applied.

        migrationBuilder.DropIndex(
            name: "UX_TenantSettings_CompanyId_Key",
            table: "TenantSettings");

        migrationBuilder.DropIndex(
            name: "IX_Depreciations_CompanyId_Name",
            table: "Depreciations");

        migrationBuilder.DropIndex(
            name: "UX_CustomFields_CompanyId_Name",
            table: "CustomFields");

        // SQL Server cannot ALTER varbinary(max) → rowversion in place; drop + re-add.
        ConvertToRowVersion(migrationBuilder, "Suppliers");
        ConvertToRowVersion(migrationBuilder, "StatusLabels");
        ConvertToRowVersion(migrationBuilder, "Manufacturers");
        ConvertToRowVersion(migrationBuilder, "Locations");
        ConvertToRowVersion(migrationBuilder, "Depreciations");
        ConvertToRowVersion(migrationBuilder, "Departments");
        ConvertToRowVersion(migrationBuilder, "Categories");
        ConvertToRowVersion(migrationBuilder, "AssetModels");

        migrationBuilder.CreateIndex(
            name: "UX_TenantSettings_CompanyId_Key",
            table: "TenantSettings",
            columns: new[] { "CompanyId", "Key" },
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_SoftwareLicenses_ManufacturerId",
            table: "SoftwareLicenses",
            column: "ManufacturerId");

        migrationBuilder.CreateIndex(
            name: "IX_ReportExports_CompanyId",
            table: "ReportExports",
            column: "CompanyId");

        migrationBuilder.CreateIndex(
            name: "IX_ReportExports_RequestedBy",
            table: "ReportExports",
            column: "RequestedBy");

        migrationBuilder.CreateIndex(
            name: "IX_LicenseSeats_AssignedAssetId",
            table: "LicenseSeats",
            column: "AssignedAssetId");

        migrationBuilder.CreateIndex(
            name: "IX_LicenseSeats_AssignedUserId",
            table: "LicenseSeats",
            column: "AssignedUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Depreciations_CompanyId_Name",
            table: "Depreciations",
            columns: new[] { "CompanyId", "Name" },
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.CreateIndex(
            name: "UX_CustomFields_CompanyId_Name",
            table: "CustomFields",
            columns: new[] { "CompanyId", "Name" },
            unique: true,
            filter: "[IsDeleted] = 0");

        migrationBuilder.AddForeignKey(
            name: "FK_Accessories_Companies_CompanyId",
            table: "Accessories",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Components_Companies_CompanyId",
            table: "Components",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Consumables_Companies_CompanyId",
            table: "Consumables",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_CustomFields_Companies_CompanyId",
            table: "CustomFields",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Depreciations_Companies_CompanyId",
            table: "Depreciations",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LicenseSeats_Assets_AssignedAssetId",
            table: "LicenseSeats",
            column: "AssignedAssetId",
            principalTable: "Assets",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_LicenseSeats_Users_AssignedUserId",
            table: "LicenseSeats",
            column: "AssignedUserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_Notifications_Companies_CompanyId",
            table: "Notifications",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_ReportExports_Companies_CompanyId",
            table: "ReportExports",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_ReportExports_Users_RequestedBy",
            table: "ReportExports",
            column: "RequestedBy",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_SoftwareLicenses_Companies_CompanyId",
            table: "SoftwareLicenses",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_SoftwareLicenses_Manufacturers_ManufacturerId",
            table: "SoftwareLicenses",
            column: "ManufacturerId",
            principalTable: "Manufacturers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_TenantSettings_Companies_CompanyId",
            table: "TenantSettings",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Accessories_Companies_CompanyId",
            table: "Accessories");

        migrationBuilder.DropForeignKey(
            name: "FK_Components_Companies_CompanyId",
            table: "Components");

        migrationBuilder.DropForeignKey(
            name: "FK_Consumables_Companies_CompanyId",
            table: "Consumables");

        migrationBuilder.DropForeignKey(
            name: "FK_CustomFields_Companies_CompanyId",
            table: "CustomFields");

        migrationBuilder.DropForeignKey(
            name: "FK_Depreciations_Companies_CompanyId",
            table: "Depreciations");

        migrationBuilder.DropForeignKey(
            name: "FK_LicenseSeats_Assets_AssignedAssetId",
            table: "LicenseSeats");

        migrationBuilder.DropForeignKey(
            name: "FK_LicenseSeats_Users_AssignedUserId",
            table: "LicenseSeats");

        migrationBuilder.DropForeignKey(
            name: "FK_Notifications_Companies_CompanyId",
            table: "Notifications");

        migrationBuilder.DropForeignKey(
            name: "FK_ReportExports_Companies_CompanyId",
            table: "ReportExports");

        migrationBuilder.DropForeignKey(
            name: "FK_ReportExports_Users_RequestedBy",
            table: "ReportExports");

        migrationBuilder.DropForeignKey(
            name: "FK_SoftwareLicenses_Companies_CompanyId",
            table: "SoftwareLicenses");

        migrationBuilder.DropForeignKey(
            name: "FK_SoftwareLicenses_Manufacturers_ManufacturerId",
            table: "SoftwareLicenses");

        migrationBuilder.DropForeignKey(
            name: "FK_TenantSettings_Companies_CompanyId",
            table: "TenantSettings");

        migrationBuilder.DropIndex(
            name: "UX_TenantSettings_CompanyId_Key",
            table: "TenantSettings");

        migrationBuilder.DropIndex(
            name: "IX_SoftwareLicenses_ManufacturerId",
            table: "SoftwareLicenses");

        migrationBuilder.DropIndex(
            name: "IX_ReportExports_CompanyId",
            table: "ReportExports");

        migrationBuilder.DropIndex(
            name: "IX_ReportExports_RequestedBy",
            table: "ReportExports");

        migrationBuilder.DropIndex(
            name: "IX_LicenseSeats_AssignedAssetId",
            table: "LicenseSeats");

        migrationBuilder.DropIndex(
            name: "IX_LicenseSeats_AssignedUserId",
            table: "LicenseSeats");

        migrationBuilder.DropIndex(
            name: "IX_Depreciations_CompanyId_Name",
            table: "Depreciations");

        migrationBuilder.DropIndex(
            name: "UX_CustomFields_CompanyId_Name",
            table: "CustomFields");

        ConvertToVarbinaryMax(migrationBuilder, "Suppliers");
        ConvertToVarbinaryMax(migrationBuilder, "StatusLabels");
        ConvertToVarbinaryMax(migrationBuilder, "Manufacturers");
        ConvertToVarbinaryMax(migrationBuilder, "Locations");
        ConvertToVarbinaryMax(migrationBuilder, "Depreciations");
        ConvertToVarbinaryMax(migrationBuilder, "Departments");
        ConvertToVarbinaryMax(migrationBuilder, "Categories");
        ConvertToVarbinaryMax(migrationBuilder, "AssetModels");

        migrationBuilder.Sql("""
            ALTER TABLE [Assets] SET (SYSTEM_VERSIONING = OFF);
            """);

        migrationBuilder.AddColumn<Guid>(
            name: "DepreciationId1",
            table: "Assets",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.Sql("""
            IF OBJECT_ID(N'[dbo].[AssetsHistory]', N'U') IS NOT NULL
               AND COL_LENGTH(N'dbo.AssetsHistory', N'DepreciationId1') IS NULL
                ALTER TABLE [AssetsHistory] ADD [DepreciationId1] uniqueidentifier NULL;

            ALTER TABLE [Assets] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[AssetsHistory]));
            """);

        migrationBuilder.CreateIndex(
            name: "UX_TenantSettings_CompanyId_Key",
            table: "TenantSettings",
            columns: new[] { "CompanyId", "Key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Depreciations_CompanyId_Name",
            table: "Depreciations",
            columns: new[] { "CompanyId", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_CustomFields_CompanyId_Name",
            table: "CustomFields",
            columns: new[] { "CompanyId", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Assets_DepreciationId1",
            table: "Assets",
            column: "DepreciationId1");

        migrationBuilder.AddForeignKey(
            name: "FK_Assets_Depreciations_DepreciationId1",
            table: "Assets",
            column: "DepreciationId1",
            principalTable: "Depreciations",
            principalColumn: "Id");
    }

    private static void ConvertToRowVersion(MigrationBuilder migrationBuilder, string table)
    {
        migrationBuilder.Sql($"""
            ALTER TABLE [{table}] DROP COLUMN [RowVersion];
            ALTER TABLE [{table}] ADD [RowVersion] rowversion NOT NULL;
            """);
    }

    private static void ConvertToVarbinaryMax(MigrationBuilder migrationBuilder, string table)
    {
        migrationBuilder.Sql($"""
            ALTER TABLE [{table}] DROP COLUMN [RowVersion];
            ALTER TABLE [{table}] ADD [RowVersion] varbinary(max) NOT NULL DEFAULT 0x;
            """);
    }
}
