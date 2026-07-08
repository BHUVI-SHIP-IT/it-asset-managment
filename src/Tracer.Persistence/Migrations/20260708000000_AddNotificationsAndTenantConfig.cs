using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class AddNotificationsAndTenantConfig : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ── Notifications table ──────────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Severity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                Channel = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Recipient = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                FailureReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                IsRead = table.Column<bool>(type: "bit", nullable: false),
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
                table.PrimaryKey("PK_Notifications", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_CompanyId_Status",
            table: "Notifications",
            columns: new[] { "CompanyId", "Status" });

        // ── TenantSettings table ─────────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "TenantSettings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                table.PrimaryKey("PK_TenantSettings", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "UX_TenantSettings_CompanyId_Key",
            table: "TenantSettings",
            columns: new[] { "CompanyId", "Key" },
            unique: true);

        // ── CustomFields table ───────────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "CustomFields",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                FieldType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                IsRequired = table.Column<bool>(type: "bit", nullable: false),
                Options = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                table.PrimaryKey("PK_CustomFields", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "UX_CustomFields_CompanyId_Name",
            table: "CustomFields",
            columns: new[] { "CompanyId", "Name" },
            unique: true);

        // ── CustomFieldValues table ──────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "CustomFieldValues",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CustomFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                table.PrimaryKey("PK_CustomFieldValues", x => x.Id);
                table.ForeignKey(
                    name: "FK_CustomFieldValues_CustomFields_CustomFieldId",
                    column: x => x.CustomFieldId,
                    principalTable: "CustomFields",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "UX_CustomFieldValues_FieldId_EntityId",
            table: "CustomFieldValues",
            columns: new[] { "CustomFieldId", "EntityId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CustomFieldValues");
        migrationBuilder.DropTable(name: "CustomFields");
        migrationBuilder.DropTable(name: "TenantSettings");
        migrationBuilder.DropTable(name: "Notifications");
    }
}
