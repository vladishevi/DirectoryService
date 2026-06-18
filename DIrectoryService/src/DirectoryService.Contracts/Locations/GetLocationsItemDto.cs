namespace DirectoryService.Contracts.Locations;

public record GetLocationsItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public AddressDto Address { get; init; }
    public int DepartmentsCount { get; init; }
}