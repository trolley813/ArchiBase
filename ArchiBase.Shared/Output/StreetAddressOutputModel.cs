namespace ArchiBase.Shared.Output;

public class StreetAddressOutputModel
{
    public Guid Id { get; set; }
    public BuildingCardOutputModel BuildingCard { get; set; }
    public StreetOutputModel Street { get; set; }
    public string HouseNumber { get; set; }

    public override string ToString()
    {
        return $"{Street.Name}, {HouseNumber}";
    }
}
