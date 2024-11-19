using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddCardAddressView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW "View_CardAddressMappings"
                AS SELECT "BuildingCardId",
                jsonb_agg(json_build_object('StreetName', s."Name", 'HouseNumber', a."HouseNumber")) as "Addresses"
                FROM "StreetAddresses" a
                JOIN "Streets" s
                ON a."StreetId" = s."Id"
                GROUP BY "BuildingCardId"
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP VIEW "View_CardAddressMappings" """);
        }
    }
}
