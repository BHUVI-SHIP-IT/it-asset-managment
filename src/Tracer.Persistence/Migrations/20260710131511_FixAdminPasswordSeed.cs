using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class FixAdminPasswordSeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
            column: "PasswordHash",
            value: "$2a$11$AeZacdZCy9zUfbQyUnd3fuVX.rG9K1Cslyg8YK5Z6tBIPiTG5V6pe");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
            column: "PasswordHash",
            value: "$2a$11$N9V2V2W41q4.F854hV5/Z.tJjU.n/q.4mO1h3Z/g71.p3z7g91/m6");
    }
}
