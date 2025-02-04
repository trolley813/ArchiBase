using ArchiBase.Data;
using ArchiBase.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Utils;

public class StreetNameService(ModelContext modelContext)
{
    public ModelContext ModelContext { get; } = modelContext;

    public Street GetStreetNameForDate(Street street, ImpreciseDate date)
    {
        var candidates = ModelContext.Streets.Include(s => s.ActualStreet).Where(s => s.ActualStreet == street).ToList();
        if (candidates.Count == 0) return street;
        var result = candidates.FirstOrDefault(s => s.From?.Date <= date.Date && s.To?.Date >= date.Date);
        if (result is not null) return result;
        result = candidates.FirstOrDefault(s => s.To?.Date >= date.Date);
        if (result is not null) return result;
        result = candidates.FirstOrDefault(s => s.From?.Date <= date.Date);
        if (result is not null) return result;
        return street;
    }
}