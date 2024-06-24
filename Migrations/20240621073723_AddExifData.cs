using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class AddExifData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Exif_AName",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_AV",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_CameraModel",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_Cameraman",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_Copy",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_DZoom",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_EFL",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exif_EMeter",
                table: "Photos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_EV",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_Editor",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_FL",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exif_Flash",
                table: "Photos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exif_ISO",
                table: "Photos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Exif_PhotoDate",
                table: "Photos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exif_SMode",
                table: "Photos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exif_TV",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exif_WB",
                table: "Photos",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exif_AName",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_AV",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_CameraModel",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_Cameraman",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_Copy",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_DZoom",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_EFL",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_EMeter",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_EV",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_Editor",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_FL",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_Flash",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_ISO",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_PhotoDate",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_SMode",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_TV",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Exif_WB",
                table: "Photos");
        }
    }
}
