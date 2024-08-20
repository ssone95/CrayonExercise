using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crayon.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Orders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Orders",
                newName: "TransactionFee");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderLines",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<double>(
                name: "GrandTotal",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalTax",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaseTotalPrice",
                table: "OrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "OrderLines",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "TaxPerUnitMultiplier",
                table: "OrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "OrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalTax",
                table: "OrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrandTotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalTax",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BaseTotalPrice",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "TaxPerUnitMultiplier",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "TotalTax",
                table: "OrderLines");

            migrationBuilder.RenameColumn(
                name: "TransactionFee",
                table: "Orders",
                newName: "Tax");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderLines",
                newName: "Price");
        }
    }
}
