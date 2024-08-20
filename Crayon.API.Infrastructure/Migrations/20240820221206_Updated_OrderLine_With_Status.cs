using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crayon.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated_OrderLine_With_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenseStatus",
                table: "OrderLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidUntil",
                table: "OrderLines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseStatus",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "ValidUntil",
                table: "OrderLines");
        }
    }
}
