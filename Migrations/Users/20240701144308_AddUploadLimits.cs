using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations.Users
{
    /// <inheritdoc />
    public partial class AddUploadLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectUploadLimitOverride",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UploadLimitOverride",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectUploadLimitOverride",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UploadLimitOverride",
                table: "AspNetUsers");
        }
    }
}
