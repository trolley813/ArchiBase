using ArchiBase.Data;
using ArchiBase.Models;
using EFMaterializedPath;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Users;

public class LocalEditorService
{
    private readonly ModelContext modelContext;
    private readonly ITreeRepository<Location, Guid> treeRepository;

    public LocalEditorService(ModelContext modelContext, UserManager<ArchiBaseUser> userManager)
    {
        this.modelContext = modelContext;
    }

    public bool CanEdit(ArchiBaseUser user, Guid locationId)
    {
        foreach (Guid userLocId in user.Locations)
        {
            // TODO: Invsetigate using of treeRepository.GetByIdAsync(userLocId)
            var location = modelContext.Locations.FirstOrDefault(loc => loc.Id == userLocId);
            if (location is not null && treeRepository.QueryDescendants(location).Select(loc => loc.Id).Contains(locationId))
            {
                return true;
            }
        }
        return false;
    }
}