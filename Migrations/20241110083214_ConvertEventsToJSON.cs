using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class ConvertEventsToJSON : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Events",
                table: "Buildings",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Buildings" b
                SET "Events" = sq.events
                FROM 
                (
                    SELECT "BuildingId", 
                    JSON_AGG(JSON_BUILD_OBJECT('Type', "Type", 'Date', 
                    JSON_BUILD_OBJECT('Date', "Date_Date", 'Precision', "Date_Precision")) 
                    ORDER BY "Date_Date") events FROM "BuildingEvents" GROUP BY "BuildingId"
                ) sq
                WHERE b."Id" = sq."BuildingId"
            """, true);

            migrationBuilder.Sql("""UPDATE "Buildings" SET "Events" = '[]' WHERE "Events" IS NULL""", true);

            migrationBuilder.DropTable(
                name: "BuildingEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "BuildingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Date_Precision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingEvents_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingEvents_BuildingId",
                table: "BuildingEvents",
                column: "BuildingId");

            migrationBuilder.DropColumn(
                name: "Events",
                table: "Buildings");
        }
    }
}
