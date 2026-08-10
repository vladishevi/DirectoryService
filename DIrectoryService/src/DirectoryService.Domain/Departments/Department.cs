using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    // EF Core
    private Department() { }
    
    [JsonConstructor]
    private Department(Name name, 
        Identifier identifier,
        Department? parentDepartment)
    {
        Path path = new(identifier, parentDepartment);
        short depth = GetDepth(parentDepartment);
        
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Name = name;
        Identifier = identifier;
        ParentDepartment = parentDepartment;
        Path = path;
        Depth = depth;
    }
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Identifier Identifier { get; private set; }
    public Department? ParentDepartment { get; private set; }
    public Path Path { get; private set; }
    public short Depth { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }
    public DateTime DeletedAt  { get; private set; }

    public IReadOnlyCollection<DepartmentLocation> Locations => _locations;
    public IReadOnlyCollection<DepartmentPosition> Positions => _positions;
    
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    public static Result<Department, Errors> Create(Name name, 
        Identifier identifier,
        Department? parentDepartment)
    {
        return new Department(name, identifier, parentDepartment);
    }

    public void UpdateLocations(IEnumerable<Guid> locationsIds)
    {
        _locations.Clear();
        _locations.AddRange(locationsIds.Select(locationId => new DepartmentLocation(this, locationId)));
    }

    public void RemoveLocation(Guid locationId)
    {
        DepartmentLocation? location = _locations.FirstOrDefault(l => l.LocationId == locationId);
        if (location != null)
        {
            _locations.Remove(location);
        }
    }

    public void UpdatePositions(IEnumerable<Guid> positionsIds)
    {
        _positions.Clear();
        _positions.AddRange(positionsIds.Select(positionId => new DepartmentPosition(Id, positionId)));
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    private static short GetDepth(Department? parentDepartment)
    {
        if (parentDepartment == null)
            return 0;

        return (short)(parentDepartment.Depth + 1);
    }
}
