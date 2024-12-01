using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoLocationView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW "View_PhotoLocationMappings"
                AS SELECT "PhotoId", "LocationId"
                FROM "BuildingBinds" bb
                INNER JOIN "Buildings" bu
                ON bb."BuildingId" = bu."Id"
                GROUP BY "PhotoId", "LocationId"
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP VIEW "View_PhotoLocationMappings" """);
        }
    }
}
