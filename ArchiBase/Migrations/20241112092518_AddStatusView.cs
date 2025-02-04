using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW "View_BuildingStatusMappings"
                AS SELECT "Id" AS "BuildingId", 
                jsonb_path_query_first("Events", '$[*] ? (@.Type == 1)')->'Date' AS "ConstructionStarted",
                jsonb_path_query_first("Events", '$[*] ? (@.Type == 2)')->'Date' AS "ConstructionFinished",
                jsonb_path_query_first("Events", '$[*] ? (@.Type == 3)')->'Date' AS "RebuildingStarted",
                jsonb_path_query_first("Events", '$[*] ? (@.Type == 4)')->'Date' AS "RebuildingFinished",
                jsonb_path_query_first("Events", '$[*] ? (@.Type == 5)')->'Date' AS "Abandoned",
                jsonb_path_query_first("Events", '$[*] ? (@.Type == 6)')->'Date' AS "Demolished"
                FROM "Buildings"
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP VIEW "View_BuildingStatusMappings" """);
        }
    }
}
