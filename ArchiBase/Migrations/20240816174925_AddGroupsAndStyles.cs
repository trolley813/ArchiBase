using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupsAndStyles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Groups_GroupId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_GroupId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Buildings");

            migrationBuilder.AddColumn<Guid>(
                name: "StyleId",
                table: "BuildingCards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BuildingGroup",
                columns: table => new
                {
                    BuildingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingGroup", x => new { x.BuildingsId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_BuildingGroup_Buildings_BuildingsId",
                        column: x => x.BuildingsId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildingGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Styles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Styles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingCards_StyleId",
                table: "BuildingCards",
                column: "StyleId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingGroup_GroupsId",
                table: "BuildingGroup",
                column: "GroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildingCards_Styles_StyleId",
                table: "BuildingCards",
                column: "StyleId",
                principalTable: "Styles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildingCards_Styles_StyleId",
                table: "BuildingCards");

            migrationBuilder.DropTable(
                name: "BuildingGroup");

            migrationBuilder.DropTable(
                name: "Styles");

            migrationBuilder.DropIndex(
                name: "IX_BuildingCards_StyleId",
                table: "BuildingCards");

            migrationBuilder.DropColumn(
                name: "StyleId",
                table: "BuildingCards");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Buildings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_GroupId",
                table: "Buildings",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Groups_GroupId",
                table: "Buildings",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
