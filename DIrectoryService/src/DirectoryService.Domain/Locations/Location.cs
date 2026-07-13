using DirectoryService.Domain.Departments;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    // EF Core
    private Location() { }
    
    public Location(Name name, Address address, Timezone timezone)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public IReadOnlyCollection<DepartmentLocation> Departments => _departments;
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Address Address { get; private set; }
    public Timezone Timezone { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }
    public DateTime DeletedAt  { get; private set; }
    
    private readonly List<DepartmentLocation> _departments = [];

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}