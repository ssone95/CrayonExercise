using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrayonCCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_ApiKeyHash_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ApiKeyHash",
                table: "ApiKeys",
                type: "varbinary(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKeyHash",
                table: "ApiKeys");
        }
    }
}
