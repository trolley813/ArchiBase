using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class WebApiUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Buildings_ActualCardId",
                table: "Buildings");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_ActualCardId",
                table: "Buildings",
                column: "ActualCardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Buildings_ActualCardId",
                table: "Buildings");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_ActualCardId",
                table: "Buildings",
                column: "ActualCardId");
        }
    }
}
