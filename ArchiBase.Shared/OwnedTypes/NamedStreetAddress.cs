using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

[Owned]
public class NamedStreetAddress
{
    public string StreetName { get; set; }
    public string HouseNumber { get; set; }

    override public string ToString() => $"{StreetName}, {HouseNumber}";
}
