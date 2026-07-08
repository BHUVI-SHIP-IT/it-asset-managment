using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class EnableTemporalAssets : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterTable(
            name: "Assets")
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalHistoryTableName", "AssetsHistory")
            .Annotation("SqlServer:TemporalHistoryTableSchema", null)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

        migrationBuilder.AddColumn<DateTime>(
            name: "PeriodEnd",
            table: "Assets",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

        migrationBuilder.AddColumn<DateTime>(
            name: "PeriodStart",
            table: "Assets",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
            .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PeriodEnd",
            table: "Assets")
            .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

        migrationBuilder.DropColumn(
            name: "PeriodStart",
            table: "Assets")
            .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

        migrationBuilder.AlterTable(
            name: "Assets")
            .OldAnnotation("SqlServer:IsTemporal", true)
            .OldAnnotation("SqlServer:TemporalHistoryTableName", "AssetsHistory")
            .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
            .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
    }
}
