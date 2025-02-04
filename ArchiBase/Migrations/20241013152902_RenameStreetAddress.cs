using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class RenameStreetAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreetAddress_BuildingCards_BuildingCardId",
                table: "StreetAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_StreetAddress_Streets_StreetId",
                table: "StreetAddress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreetAddress",
                table: "StreetAddress");

            migrationBuilder.RenameTable(
                name: "StreetAddress",
                newName: "StreetAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_StreetAddress_StreetId",
                table: "StreetAddresses",
                newName: "IX_StreetAddresses_StreetId");

            migrationBuilder.RenameIndex(
                name: "IX_StreetAddress_BuildingCardId",
                table: "StreetAddresses",
                newName: "IX_StreetAddresses_BuildingCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreetAddresses",
                table: "StreetAddresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StreetAddresses_BuildingCards_BuildingCardId",
                table: "StreetAddresses",
                column: "BuildingCardId",
                principalTable: "BuildingCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreetAddresses_Streets_StreetId",
                table: "StreetAddresses",
                column: "StreetId",
                principalTable: "Streets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreetAddresses_BuildingCards_BuildingCardId",
                table: "StreetAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StreetAddresses_Streets_StreetId",
                table: "StreetAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreetAddresses",
                table: "StreetAddresses");

            migrationBuilder.RenameTable(
                name: "StreetAddresses",
                newName: "StreetAddress");

            migrationBuilder.RenameIndex(
                name: "IX_StreetAddresses_StreetId",
                table: "StreetAddress",
                newName: "IX_StreetAddress_StreetId");

            migrationBuilder.RenameIndex(
                name: "IX_StreetAddresses_BuildingCardId",
                table: "StreetAddress",
                newName: "IX_StreetAddress_BuildingCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreetAddress",
                table: "StreetAddress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StreetAddress_BuildingCards_BuildingCardId",
                table: "StreetAddress",
                column: "BuildingCardId",
                principalTable: "BuildingCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreetAddress_Streets_StreetId",
                table: "StreetAddress",
                column: "StreetId",
                principalTable: "Streets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
