namespace DirectoryService.Contracts.Locations;

public record GetLocationDto
{
    public IEnumerable<Guid> DepartmentsIds { get; init; }
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string City { get; init; }
    public string Street { get; init; }
    public int Building { get; init; }
    public string Postcode { get; init; }
    public string Timezone { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt  { get; init; }
    public DateTime UpdatedAt  { get; init; }
}