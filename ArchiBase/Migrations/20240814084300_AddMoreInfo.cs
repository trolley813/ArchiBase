using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Architects_BuildingCards_BuildingCardId",
                table: "Architects");

            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Architects_ArchitectId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Architects_BuildingCardId",
                table: "Architects");

            migrationBuilder.DropColumn(
                name: "BuildingCardId",
                table: "Architects");

            migrationBuilder.RenameColumn(
                name: "ArchitectId",
                table: "Buildings",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Buildings_ArchitectId",
                table: "Buildings",
                newName: "IX_Buildings_GroupId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Streets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "From_Date",
                table: "Streets",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "From_Precision",
                table: "Streets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "To_Date",
                table: "Streets",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "To_Precision",
                table: "Streets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArchitectBuildingCard",
                columns: table => new
                {
                    ArchitectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingCardsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchitectBuildingCard", x => new { x.ArchitectsId, x.BuildingCardsId });
                    table.ForeignKey(
                        name: "FK_ArchitectBuildingCard_Architects_ArchitectsId",
                        column: x => x.ArchitectsId,
                        principalTable: "Architects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchitectBuildingCard_BuildingCards_BuildingCardsId",
                        column: x => x.BuildingCardsId,
                        principalTable: "BuildingCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchitectBuildingCard_BuildingCardsId",
                table: "ArchitectBuildingCard",
                column: "BuildingCardsId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LocationId",
                table: "Groups",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Groups_GroupId",
                table: "Buildings",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Groups_GroupId",
                table: "Buildings");

            migrationBuilder.DropTable(
                name: "ArchitectBuildingCard");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Streets");

            migrationBuilder.DropColumn(
                name: "From_Date",
                table: "Streets");

            migrationBuilder.DropColumn(
                name: "From_Precision",
                table: "Streets");

            migrationBuilder.DropColumn(
                name: "To_Date",
                table: "Streets");

            migrationBuilder.DropColumn(
                name: "To_Precision",
                table: "Streets");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Buildings",
                newName: "ArchitectId");

            migrationBuilder.RenameIndex(
                name: "IX_Buildings_GroupId",
                table: "Buildings",
                newName: "IX_Buildings_ArchitectId");

            migrationBuilder.AddColumn<Guid>(
                name: "BuildingCardId",
                table: "Architects",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Architects_BuildingCardId",
                table: "Architects",
                column: "BuildingCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Architects_BuildingCards_BuildingCardId",
                table: "Architects",
                column: "BuildingCardId",
                principalTable: "BuildingCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Architects_ArchitectId",
                table: "Buildings",
                column: "ArchitectId",
                principalTable: "Architects",
                principalColumn: "Id");
        }
    }
}
