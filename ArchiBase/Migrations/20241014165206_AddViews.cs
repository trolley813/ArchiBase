using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW "View_PhotoAuthorMappings"
                AS SELECT "Photos"."Id" AS "PhotoId", "AspNetUsers"."UserName" as "AuthorName"
                FROM "Photos" LEFT JOIN "AspNetUsers"
                ON "Photos"."AuthorId" = "AspNetUsers"."Id"
            """);

            migrationBuilder.Sql("""
                CREATE VIEW "View_CommentAuthorMappings"
                AS SELECT "Comments"."Id" AS "CommentId", "AspNetUsers"."UserName" as "AuthorName"
                FROM "Comments" LEFT JOIN "AspNetUsers"
                ON "Comments"."AuthorId" = "AspNetUsers"."Id"
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP VIEW "View_CommentAuthorMappings" """);
            migrationBuilder.Sql("""DROP VIEW "View_PhotoAuthorMappings" """);
        }
    }
}
