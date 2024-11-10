using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddActualCardField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActualCardId",
                table: "Buildings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_ActualCardId",
                table: "Buildings",
                column: "ActualCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_BuildingCards_ActualCardId",
                table: "Buildings",
                column: "ActualCardId",
                principalTable: "BuildingCards",
                principalColumn: "Id");

            migrationBuilder.Sql("""
                UPDATE "Buildings" b
                SET "ActualCardId" = sq."Id"
                FROM (SELECT c1."Id", c1."BuildingId" FROM "BuildingCards" c1
                JOIN (
                SELECT "BuildingId", MAX("ActualFrom_Date") maxdate FROM "BuildingCards" GROUP BY "BuildingId"
                ) c2 
                ON c1."BuildingId" = c2."BuildingId" AND c1."ActualFrom_Date" = c2.maxdate) sq
                WHERE b."Id" = sq."BuildingId"
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_BuildingCards_ActualCardId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_ActualCardId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "ActualCardId",
                table: "Buildings");
        }
    }
}
