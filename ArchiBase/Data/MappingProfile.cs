using ArchiBase.Models;
using ArchiBase.Shared.Input;
using ArchiBase.Shared.Output;
using ArchiBase.Users;
using AutoMapper;

namespace ArchiBase.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Architectural
        CreateMap<DesignCategoryInputModel, DesignCategory>();
        CreateMap<DesignCategory, DesignCategoryOutputModel>();
        CreateMap<DesignCategory, DesignCategoryBasicOutputModel>();
        CreateMap<DesignCatalogueInputModel, DesignCatalogue>();
        CreateMap<DesignCatalogue, DesignCatalogueOutputModel>();
        CreateMap<DesignCatalogueEntryInputModel, DesignCatalogueEntry>();
        CreateMap<DesignCatalogueEntry, DesignCatalogueEntryOutputModel>();
        CreateMap<DesignInputModel, Design>();
        CreateMap<Design, DesignOutputModel>();
        CreateMap<Design, DesignBasicOutputModel>();
        CreateMap<ArchitectInputModel, Architect>();
        CreateMap<Architect, ArchitectOutputModel>();
        CreateMap<Architect, ArchitectBasicOutputModel>();
        CreateMap<StyleInputModel, Style>();
        CreateMap<Style, StyleOutputModel>();
        // Audit -- probably don't need
        // Buildings
        CreateMap<LocationInputModel, Location>();
        CreateMap<Location, LocationOutputModel>();
        CreateMap<Location, LocationBasicOutputModel>();
        CreateMap<StreetInputModel, Street>();
        CreateMap<Street, StreetOutputModel>();
        CreateMap<Street, StreetBasicOutputModel>();
        CreateMap<StreetAddressInputModel, StreetAddress>();
        CreateMap<StreetAddress, StreetAddressOutputModel>();
        CreateMap<BuildingCardInputModel, BuildingCard>();
        CreateMap<BuildingCard, BuildingCardOutputModel>();
        CreateMap<BuildingPartInputModel, BuildingPart>();
        CreateMap<BuildingPart, BuildingPartOutputModel>();
        CreateMap<BuildingInputModel, Building>();
        CreateMap<Building, BuildingOutputModel>();
        CreateMap<GroupInputModel, Group>();
        CreateMap<Group, GroupOutputModel>();
        // Comments
        CreateMap<CommentInputModel, Comment>();
        CreateMap<Comment, CommentOutputModel>();
        // News
        // Pages
        // Photos
        //CreateMap<LicenseInputModel, License>();
        CreateMap<License, LicenseOutputModel>();
        CreateMap<BuildingBindInputModel, BuildingBind>();
        CreateMap<BuildingBind, BuildingBindOutputModel>();
        CreateMap<GalleryInputModel, Gallery>();
        CreateMap<Gallery, GalleryOutputModel>();
        CreateMap<PhotoInputModel, Photo>();
        CreateMap<Photo, PhotoOutputModel>();
        // Translatable - probably don't need
        // Views - probably don't need
        // Users
        CreateMap<ArchiBaseUser, UserOutputModel>();
    }
}
