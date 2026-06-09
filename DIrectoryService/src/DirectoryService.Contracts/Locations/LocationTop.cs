namespace DirectoryService.Contracts.Locations;

public record LocationTopDto()
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public AddressDto AddressDto { get; init; }
    public int DepartmentsCount { get; init; }
}