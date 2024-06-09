using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddArchitect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Architects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AbbreviatedName = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateOfBirth_Precision = table.Column<int>(type: "integer", nullable: true),
                    DateOfDeath_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateOfDeath_Precision = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Architects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArchitectBuilding",
                columns: table => new
                {
                    ArchitectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchitectBuilding", x => new { x.ArchitectsId, x.BuildingsId });
                    table.ForeignKey(
                        name: "FK_ArchitectBuilding_Architects_ArchitectsId",
                        column: x => x.ArchitectsId,
                        principalTable: "Architects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchitectBuilding_Buildings_BuildingsId",
                        column: x => x.BuildingsId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArchitectDesign",
                columns: table => new
                {
                    ArchitectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchitectDesign", x => new { x.ArchitectsId, x.DesignsId });
                    table.ForeignKey(
                        name: "FK_ArchitectDesign_Architects_ArchitectsId",
                        column: x => x.ArchitectsId,
                        principalTable: "Architects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchitectDesign_Designs_DesignsId",
                        column: x => x.DesignsId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchitectBuilding_BuildingsId",
                table: "ArchitectBuilding",
                column: "BuildingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchitectDesign_DesignsId",
                table: "ArchitectDesign",
                column: "DesignsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchitectBuilding");

            migrationBuilder.DropTable(
                name: "ArchitectDesign");

            migrationBuilder.DropTable(
                name: "Architects");
        }
    }
}
