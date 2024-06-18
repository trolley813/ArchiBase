using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchiBase.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignCatalogues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AbbreviatedFormat = table.Column<string>(type: "text", nullable: false, defaultValue: "{Code}")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignCatalogues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Designs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TotalArea = table.Column<decimal>(type: "numeric", nullable: true),
                    BuildingArea = table.Column<decimal>(type: "numeric", nullable: true),
                    Cubage = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gallery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gallery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Flag = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllowStreets = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    LanguageCode = table.Column<string>(type: "text", nullable: false),
                    TranslatedText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vote = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentVotes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DesignCatalogueEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignId = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignCatalogueEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DesignCatalogueEntries_DesignCatalogues_CatalogueId",
                        column: x => x.CatalogueId,
                        principalTable: "DesignCatalogues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DesignCatalogueEntries_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DesignDesignCategory",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignDesignCategory", x => new { x.CategoriesId, x.DesignsId });
                    table.ForeignKey(
                        name: "FK_DesignDesignCategory_DesignCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "DesignCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DesignDesignCategory_Designs_DesignsId",
                        column: x => x.DesignsId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    NonAuthor = table.Column<bool>(type: "boolean", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false, defaultValue: "jpg"),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ShootingDate_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ShootingDate_Precision = table.Column<int>(type: "integer", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    LicenseId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Streets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Streets_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GalleryPhoto",
                columns: table => new
                {
                    GalleriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotosId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryPhoto", x => new { x.GalleriesId, x.PhotosId });
                    table.ForeignKey(
                        name: "FK_GalleryPhoto_Gallery_GalleriesId",
                        column: x => x.GalleriesId,
                        principalTable: "Gallery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GalleryPhoto_Photos_PhotosId",
                        column: x => x.PhotosId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoVote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vote = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoVote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoVote_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArchitectDesign",
                columns: table => new
                {
                    ArchitectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchitectDesign", x => new { x.ArchitectsId, x.DesignsId });
                    table.ForeignKey(
                        name: "FK_ArchitectDesign_Designs_DesignsId",
                        column: x => x.DesignsId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Architects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AbbreviatedName = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateOfBirth_Precision = table.Column<int>(type: "integer", nullable: true),
                    DateOfDeath_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateOfDeath_Precision = table.Column<int>(type: "integer", nullable: true),
                    BuildingCardId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Architects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArchitectId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buildings_Architects_ArchitectId",
                        column: x => x.ArchitectId,
                        principalTable: "Architects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Buildings_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingBinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Markup_X = table.Column<int>(type: "integer", nullable: true),
                    Markup_Y = table.Column<int>(type: "integer", nullable: true),
                    Markup_Width = table.Column<int>(type: "integer", nullable: true),
                    Markup_Height = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingBinds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingBinds_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildingBinds_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FloorCount = table.Column<List<int>>(type: "integer[]", nullable: false),
                    ActualFrom_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualFrom_Precision = table.Column<int>(type: "integer", nullable: false),
                    Customer = table.Column<string>(type: "text", nullable: true),
                    Builder = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingCards_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date_Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Date_Precision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingEvents_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingCardDesignCategory",
                columns: table => new
                {
                    BuildingCardsId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingCardDesignCategory", x => new { x.BuildingCardsId, x.CategoriesId });
                    table.ForeignKey(
                        name: "FK_BuildingCardDesignCategory_BuildingCards_BuildingCardsId",
                        column: x => x.BuildingCardsId,
                        principalTable: "BuildingCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildingCardDesignCategory_DesignCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "DesignCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingPart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingPart_BuildingCards_BuildingCardId",
                        column: x => x.BuildingCardId,
                        principalTable: "BuildingCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildingPart_Designs_DesignId",
                        column: x => x.DesignId,
                        principalTable: "Designs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreetAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreetId = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreetAddress_BuildingCards_BuildingCardId",
                        column: x => x.BuildingCardId,
                        principalTable: "BuildingCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreetAddress_Streets_StreetId",
                        column: x => x.StreetId,
                        principalTable: "Streets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchitectDesign_DesignsId",
                table: "ArchitectDesign",
                column: "DesignsId");

            migrationBuilder.CreateIndex(
                name: "IX_Architects_BuildingCardId",
                table: "Architects",
                column: "BuildingCardId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingBinds_BuildingId",
                table: "BuildingBinds",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingBinds_PhotoId",
                table: "BuildingBinds",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingCardDesignCategory_CategoriesId",
                table: "BuildingCardDesignCategory",
                column: "CategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingCards_BuildingId",
                table: "BuildingCards",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingEvents_BuildingId",
                table: "BuildingEvents",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPart_BuildingCardId",
                table: "BuildingPart",
                column: "BuildingCardId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingPart_DesignId",
                table: "BuildingPart",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_ArchitectId",
                table: "Buildings",
                column: "ArchitectId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_LocationId",
                table: "Buildings",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVotes_CommentId",
                table: "CommentVotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignCatalogueEntries_CatalogueId",
                table: "DesignCatalogueEntries",
                column: "CatalogueId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignCatalogueEntries_DesignId",
                table: "DesignCatalogueEntries",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignDesignCategory_DesignsId",
                table: "DesignDesignCategory",
                column: "DesignsId");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryPhoto_PhotosId",
                table: "GalleryPhoto",
                column: "PhotosId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_LicenseId",
                table: "Photos",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoVote_PhotoId",
                table: "PhotoVote",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetAddress_BuildingCardId",
                table: "StreetAddress",
                column: "BuildingCardId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetAddress_StreetId",
                table: "StreetAddress",
                column: "StreetId");

            migrationBuilder.CreateIndex(
                name: "IX_Streets_LocationId",
                table: "Streets",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchitectDesign_Architects_ArchitectsId",
                table: "ArchitectDesign",
                column: "ArchitectsId",
                principalTable: "Architects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Architects_BuildingCards_BuildingCardId",
                table: "Architects",
                column: "BuildingCardId",
                principalTable: "BuildingCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Architects_ArchitectId",
                table: "Buildings");

            migrationBuilder.DropTable(
                name: "ArchitectDesign");

            migrationBuilder.DropTable(
                name: "AuditRecords");

            migrationBuilder.DropTable(
                name: "BuildingBinds");

            migrationBuilder.DropTable(
                name: "BuildingCardDesignCategory");

            migrationBuilder.DropTable(
                name: "BuildingEvents");

            migrationBuilder.DropTable(
                name: "BuildingPart");

            migrationBuilder.DropTable(
                name: "CommentVotes");

            migrationBuilder.DropTable(
                name: "DesignCatalogueEntries");

            migrationBuilder.DropTable(
                name: "DesignDesignCategory");

            migrationBuilder.DropTable(
                name: "GalleryPhoto");

            migrationBuilder.DropTable(
                name: "NewsItems");

            migrationBuilder.DropTable(
                name: "PhotoVote");

            migrationBuilder.DropTable(
                name: "StreetAddress");

            migrationBuilder.DropTable(
                name: "Translation");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DesignCatalogues");

            migrationBuilder.DropTable(
                name: "DesignCategories");

            migrationBuilder.DropTable(
                name: "Designs");

            migrationBuilder.DropTable(
                name: "Gallery");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Streets");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "Architects");

            migrationBuilder.DropTable(
                name: "BuildingCards");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
