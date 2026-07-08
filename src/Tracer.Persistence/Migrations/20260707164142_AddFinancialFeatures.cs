using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class AddFinancialFeatures : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "CurrentValue",
            table: "Assets",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<Guid>(
            name: "DepreciationId",
            table: "Assets",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Depreciations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Months = table.Column<int>(type: "int", nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MinimumValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                table.PrimaryKey("PK_Depreciations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ReportExports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReportName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReportExports", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Assets_DepreciationId",
            table: "Assets",
            column: "DepreciationId");

        migrationBuilder.CreateIndex(
            name: "IX_Depreciations_CompanyId_Name",
            table: "Depreciations",
            columns: new[] { "CompanyId", "Name" },
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Assets_Depreciations_DepreciationId",
            table: "Assets",
            column: "DepreciationId",
            principalTable: "Depreciations",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Assets_Depreciations_DepreciationId",
            table: "Assets");

        migrationBuilder.DropTable(
            name: "Depreciations");

        migrationBuilder.DropTable(
            name: "ReportExports");

        migrationBuilder.DropIndex(
            name: "IX_Assets_DepreciationId",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "CurrentValue",
            table: "Assets");

        migrationBuilder.DropColumn(
            name: "DepreciationId",
            table: "Assets");
    }
}
