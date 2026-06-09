namespace DirectoryService.Contracts.Locations;

public record LocationTopDto()
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required AddressDto Address { get; init; }
    public int DepartmentsCount { get; init; }
}
