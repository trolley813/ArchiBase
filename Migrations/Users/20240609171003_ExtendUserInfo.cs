using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations.Users
{
    /// <inheritdoc />
    public partial class ExtendUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadCommentTime",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Locations",
                table: "AspNetUsers",
                type: "uuid[]",
                defaultValue: new List<Guid>(),
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadCommentTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Locations",
                table: "AspNetUsers");
        }
    }
}
