using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class SetOptionalDesignToEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignCatalogueEntries_Designs_DesignId",
                table: "DesignCatalogueEntries");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "DesignCatalogueEntries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignCatalogueEntries_Designs_DesignId",
                table: "DesignCatalogueEntries",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignCatalogueEntries_Designs_DesignId",
                table: "DesignCatalogueEntries");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "DesignCatalogueEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DesignCatalogueEntries_Designs_DesignId",
                table: "DesignCatalogueEntries",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
