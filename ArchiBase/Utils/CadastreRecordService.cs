using ArchiBase.Models;
using EFMaterializedPath;
using SmartFormat;

namespace Archibase.Utils;

public class CadastreRecordService(ITreeRepository<Location, Guid> treeRepository)
{
    private readonly ITreeRepository<Location, Guid> treeRepository = treeRepository;

    static readonly Dictionary<string, string> linksByCountry = new()
    {
        ["ru"] = "https://egrp365.org/reestr?egrp={Number}",
        ["ee"] = "https://livekluster.ehr.ee/ui/ehr/v1/building/{Number}",
    };

    public string? GetLink(Building building)
    {
        if (building is null || building.CadastreRecordNumber == null)
        {
            return null;
        }
        var ancestors = treeRepository.QueryAncestors(building.Location).Select(a => a.Flag);
        foreach (var (k, v) in linksByCountry)
        {
            if (building.Location.Flag == k || ancestors.Contains(k))
            {
                return Smart.Format(v, new { Number = building.CadastreRecordNumber });
            }
        }
        return null;
    }
}