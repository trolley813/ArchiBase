namespace ArchiBase.Shared.Input;

public class StreetAddressInputModel
{
    public Guid Id { get; set; }
    public Guid BuildingCardId { get; set; }
    public Guid StreetId { get; set; }
    public string HouseNumber { get; set; }
}