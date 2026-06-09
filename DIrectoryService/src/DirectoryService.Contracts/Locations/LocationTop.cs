namespace DirectoryService.Contracts.Locations;

public record LocationTopDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public AddressDto Address { get; init; }
    public int DepartmentsCount { get; init; }
}
