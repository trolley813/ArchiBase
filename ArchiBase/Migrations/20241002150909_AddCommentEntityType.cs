using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentEntityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Comments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Comments");
        }
    }
}
