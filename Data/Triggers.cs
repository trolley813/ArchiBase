using ArchiBase.Models;
using EntityFrameworkCore.Triggered;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Data;

public class SaveBuildingTrigger(ModelContext modelContext) : IAfterSaveTrigger<Building>
{
    private readonly ModelContext modelContext = modelContext;

    public async Task AfterSave(ITriggerContext<Building> context, CancellationToken cancellationToken)
    {
        var building = context.Entity;
        modelContext.Entry(building).Collection(b => b.Cards).Load();
        building.ActualCard = building.Cards.MaxBy(c => c.ActualFrom.Date);
        await modelContext.SaveChangesAsync(cancellationToken);
    }
}

public class SaveActualCardTrigger(ModelContext modelContext) : IAfterSaveTrigger<BuildingCard>
{
    private readonly ModelContext modelContext = modelContext;

    public async Task AfterSave(ITriggerContext<BuildingCard> context, CancellationToken cancellationToken)
    {
        var building = context.Entity.Building;
        modelContext.Entry(building).Collection(b => b.Cards).Load();
        building.ActualCard = building.Cards.MaxBy(c => c.ActualFrom.Date);
        await modelContext.SaveChangesAsync(cancellationToken);
    }
}

public class CascadeOwnedChangesTrigger(
    ModelContext context
    ) : ITriggerPriority, IBeforeSaveTrigger<object>
{
    public int Priority => CommonTriggerPriority.Earlier - 1;

    private readonly ModelContext _context = context;

    public async Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken)
    {
        var entity = context.Entity;
        var entry = _context.Entry(entity);

        if (entry.Metadata.IsOwned())
        {
            var ownership = entry.Metadata.FindOwnership()!;
            var navigation = ownership.GetNavigation(false)!;
            var type = navigation.DeclaringType.ClrType;
            var propertyName = navigation.Name;
            var property = type.GetProperty(propertyName)!;

            var newEntries = _context.ChangeTracker.Entries();

            var ownerTypeEntries = newEntries.Where(e => e.Entity.GetType() == type);
            var ownerEntry = ownerTypeEntries.FirstOrDefault(e => property.GetValue(e.Entity) == entry.Entity);

            if (ownerEntry is not null && ownerEntry.State == EntityState.Unchanged)
            {
                ownerEntry.State = EntityState.Modified;
            }
        }
    }
}