using DirectoryService.Domain.Common;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;

namespace DirectoryService.Domain.Departments;

public class Department
{
    public Department(Name name, 
        Identifier identifier,
        Department? parentDepartment)
    {
        Path path = new(identifier, parentDepartment);
        short depth = CalcDepth(parentDepartment);
        
        Id = Guid.NewGuid();
        IsActive = true;
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
    public bool IsActive { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }

    public IReadOnlyCollection<DepartmentLocation> Locations => _locations;
    public IReadOnlyCollection<DepartmentPosition> Positions => _positions;
    
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    public void AddLocation(Guid locationId) => 
        _locations.Add(new DepartmentLocation(Id, locationId));
    
    public void AddPosition(Guid positionId) => 
        _positions.Add(new DepartmentPosition(Id, positionId));
    
    private static short CalcDepth(Department? parentDepartment)
    {
        short depth = 1;

        Department? parent = parentDepartment;
        while (parent != null)
        {
            depth++;
            parent = parent.ParentDepartment;
        }

        return depth;
    }
}