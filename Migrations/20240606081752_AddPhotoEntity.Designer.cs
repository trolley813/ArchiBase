﻿// <auto-generated />
using System;
using ArchiBase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArchiBase.Migrations
{
    [DbContext(typeof(ModelContext))]
    [Migration("20240606081752_AddPhotoEntity")]
    partial class AddPhotoEntity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArchiBase.Models.AuditRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("ArchiBase.Models.Building", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uuid");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingBind", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PhotoId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.HasIndex("PhotoId");

                    b.ToTable("BuildingBinds");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingCard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DesignId")
                        .HasColumnType("uuid");

                    b.Property<string>("FloorCount")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.HasIndex("DesignId");

                    b.ToTable("BuildingCards");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("BuildingEvents");
                });

            modelBuilder.Entity("ArchiBase.Models.Design", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal?>("BuildingArea")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("Cubage")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("TotalArea")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Designs");
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCatalogue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AbbreviatedFormat")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("{Code}");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DesignCatalogues");
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCatalogueEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CatalogueId")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("DesignId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CatalogueId");

                    b.HasIndex("DesignId");

                    b.ToTable("DesignCatalogueEntries");
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BuildingCardId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BuildingCardId");

                    b.HasIndex("ParentId");

                    b.ToTable("DesignCategories");
                });

            modelBuilder.Entity("ArchiBase.Models.Gallery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Gallery");
                });

            modelBuilder.Entity("ArchiBase.Models.License", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Licenses");
                });

            modelBuilder.Entity("ArchiBase.Models.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Flag")
                        .HasColumnType("text");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("ArchiBase.Models.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("Direction")
                        .HasColumnType("integer");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<Guid?>("LicenseId")
                        .HasColumnType("uuid");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<bool>("NonAuthor")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("LicenseId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("ArchiBase.Models.Street", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Streets");
                });

            modelBuilder.Entity("ArchiBase.Models.StreetAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BuildingCardId")
                        .HasColumnType("uuid");

                    b.Property<string>("HouseNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("StreetId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BuildingCardId");

                    b.HasIndex("StreetId");

                    b.ToTable("StreetAddress");
                });

            modelBuilder.Entity("DesignDesignCategory", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DesignsId")
                        .HasColumnType("uuid");

                    b.HasKey("CategoriesId", "DesignsId");

                    b.HasIndex("DesignsId");

                    b.ToTable("DesignDesignCategory");
                });

            modelBuilder.Entity("GalleryPhoto", b =>
                {
                    b.Property<Guid>("GalleriesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PhotosId")
                        .HasColumnType("uuid");

                    b.HasKey("GalleriesId", "PhotosId");

                    b.HasIndex("PhotosId");

                    b.ToTable("GalleryPhoto");
                });

            modelBuilder.Entity("Translation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FieldName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LanguageCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uuid");

                    b.Property<string>("TranslatedText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Translation");
                });

            modelBuilder.Entity("ArchiBase.Models.Building", b =>
                {
                    b.HasOne("ArchiBase.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingBind", b =>
                {
                    b.HasOne("ArchiBase.Models.Building", "Building")
                        .WithMany()
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArchiBase.Models.Photo", "Photo")
                        .WithMany("BuildingBinds")
                        .HasForeignKey("PhotoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("ArchiBase.Models.BindMarkup", "Markup", b1 =>
                        {
                            b1.Property<Guid>("BuildingBindId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Height")
                                .HasColumnType("integer");

                            b1.Property<int>("Width")
                                .HasColumnType("integer");

                            b1.Property<int>("X")
                                .HasColumnType("integer");

                            b1.Property<int>("Y")
                                .HasColumnType("integer");

                            b1.HasKey("BuildingBindId");

                            b1.ToTable("BuildingBinds");

                            b1.WithOwner()
                                .HasForeignKey("BuildingBindId");
                        });

                    b.Navigation("Building");

                    b.Navigation("Markup");

                    b.Navigation("Photo");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingCard", b =>
                {
                    b.HasOne("ArchiBase.Models.Building", "Building")
                        .WithMany("Cards")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArchiBase.Models.Design", "Design")
                        .WithMany()
                        .HasForeignKey("DesignId");

                    b.OwnsOne("ArchiBase.Models.ImpreciseDate", "ActualFrom", b1 =>
                        {
                            b1.Property<Guid>("BuildingCardId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("timestamp without time zone");

                            b1.Property<int>("Precision")
                                .HasColumnType("integer");

                            b1.HasKey("BuildingCardId");

                            b1.ToTable("BuildingCards");

                            b1.WithOwner()
                                .HasForeignKey("BuildingCardId");
                        });

                    b.Navigation("ActualFrom")
                        .IsRequired();

                    b.Navigation("Building");

                    b.Navigation("Design");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingEvent", b =>
                {
                    b.HasOne("ArchiBase.Models.Building", "Building")
                        .WithMany("Events")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("ArchiBase.Models.ImpreciseDate", "Date", b1 =>
                        {
                            b1.Property<Guid>("BuildingEventId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("timestamp without time zone");

                            b1.Property<int>("Precision")
                                .HasColumnType("integer");

                            b1.HasKey("BuildingEventId");

                            b1.ToTable("BuildingEvents");

                            b1.WithOwner()
                                .HasForeignKey("BuildingEventId");
                        });

                    b.Navigation("Building");

                    b.Navigation("Date")
                        .IsRequired();
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCatalogueEntry", b =>
                {
                    b.HasOne("ArchiBase.Models.DesignCatalogue", "Catalogue")
                        .WithMany("Entries")
                        .HasForeignKey("CatalogueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArchiBase.Models.Design", "Design")
                        .WithMany("CatalogueEntries")
                        .HasForeignKey("DesignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Catalogue");

                    b.Navigation("Design");
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCategory", b =>
                {
                    b.HasOne("ArchiBase.Models.BuildingCard", null)
                        .WithMany("Categories")
                        .HasForeignKey("BuildingCardId");

                    b.HasOne("ArchiBase.Models.DesignCategory", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("ArchiBase.Models.Location", b =>
                {
                    b.HasOne("ArchiBase.Models.Location", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("ArchiBase.Models.Photo", b =>
                {
                    b.HasOne("ArchiBase.Models.License", "License")
                        .WithMany()
                        .HasForeignKey("LicenseId");

                    b.OwnsOne("ArchiBase.Models.ImpreciseDate", "ShootingDate", b1 =>
                        {
                            b1.Property<Guid>("PhotoId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("timestamp without time zone");

                            b1.Property<int>("Precision")
                                .HasColumnType("integer");

                            b1.HasKey("PhotoId");

                            b1.ToTable("Photos");

                            b1.WithOwner()
                                .HasForeignKey("PhotoId");
                        });

                    b.Navigation("License");

                    b.Navigation("ShootingDate")
                        .IsRequired();
                });

            modelBuilder.Entity("ArchiBase.Models.Street", b =>
                {
                    b.HasOne("ArchiBase.Models.Location", "Location")
                        .WithMany("Streets")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("ArchiBase.Models.StreetAddress", b =>
                {
                    b.HasOne("ArchiBase.Models.BuildingCard", "BuildingCard")
                        .WithMany("StreetAddresses")
                        .HasForeignKey("BuildingCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArchiBase.Models.Street", "Street")
                        .WithMany()
                        .HasForeignKey("StreetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BuildingCard");

                    b.Navigation("Street");
                });

            modelBuilder.Entity("DesignDesignCategory", b =>
                {
                    b.HasOne("ArchiBase.Models.DesignCategory", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArchiBase.Models.Design", null)
                        .WithMany()
                        .HasForeignKey("DesignsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GalleryPhoto", b =>
                {
                    b.HasOne("ArchiBase.Models.Gallery", null)
                        .WithMany()
                        .HasForeignKey("GalleriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArchiBase.Models.Photo", null)
                        .WithMany()
                        .HasForeignKey("PhotosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArchiBase.Models.Building", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Events");
                });

            modelBuilder.Entity("ArchiBase.Models.BuildingCard", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("StreetAddresses");
                });

            modelBuilder.Entity("ArchiBase.Models.Design", b =>
                {
                    b.Navigation("CatalogueEntries");
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCatalogue", b =>
                {
                    b.Navigation("Entries");
                });

            modelBuilder.Entity("ArchiBase.Models.DesignCategory", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("ArchiBase.Models.Location", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Streets");
                });

            modelBuilder.Entity("ArchiBase.Models.Photo", b =>
                {
                    b.Navigation("BuildingBinds");
                });
#pragma warning restore 612, 618
        }
    }
}
