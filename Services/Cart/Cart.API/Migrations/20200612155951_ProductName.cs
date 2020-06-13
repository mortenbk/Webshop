using Microsoft.EntityFrameworkCore.Migrations;

namespace Cart.API.Migrations
{
    public partial class ProductName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CartItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CartItems");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
