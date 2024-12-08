using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddActualStreetLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActualStreetId",
                table: "Streets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streets_ActualStreetId",
                table: "Streets",
                column: "ActualStreetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Streets_Streets_ActualStreetId",
                table: "Streets",
                column: "ActualStreetId",
                principalTable: "Streets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Streets_ActualStreetId",
                table: "Streets");

            migrationBuilder.DropIndex(
                name: "IX_Streets_ActualStreetId",
                table: "Streets");

            migrationBuilder.DropColumn(
                name: "ActualStreetId",
                table: "Streets");
        }
    }
}
