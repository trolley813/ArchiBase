using ArchiBase.Data;
using ArchiBase.Models;
using EFMaterializedPath;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Users;

public class LocalEditorService
{
    private readonly ModelContext modelContext;
    private readonly UserManager<ArchiBaseUser> userManager;
    private readonly ITreeRepository<Location, Guid> treeRepository;

    public LocalEditorService(ModelContext modelContext, ITreeRepository<Location, Guid> treeRepository, UserManager<ArchiBaseUser> userManager)
    {
        this.modelContext = modelContext;
        this.treeRepository = treeRepository;
        this.userManager = userManager;
    }

    public bool CanEdit(ArchiBaseUser? user, Guid locationId)
    {
        if (user is null) return false;

        foreach (Guid userLocId in user.Locations)
        {
            if (userLocId == locationId)
            {
                return true;
            }
            // TODO: Investigate using of treeRepository.GetByIdAsync(userLocId)
            var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == userLocId);
            if (location is not null && treeRepository.QueryDescendants(location).Select(loc => loc.Id).Contains(locationId))
            {
                return true;
            }
        }
        return false;
    }
}