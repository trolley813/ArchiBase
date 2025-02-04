using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddBindingOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "BuildingBinds",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "BuildingBinds");
        }
    }
}
