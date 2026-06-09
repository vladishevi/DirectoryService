namespace DirectoryService.Contracts.Locations;


public record AddressDto
{
    public string City { get; init; }
    public string Street { get; init; }
    public int Building { get; init; }
    public string Postcode { get; init; }
}
