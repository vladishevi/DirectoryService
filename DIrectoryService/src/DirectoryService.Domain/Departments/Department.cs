using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    // EF Core
    private Department() { }
    
    private Department(Name name, 
        Identifier identifier,
        Department? parentDepartment,
        IEnumerable<DepartmentLocation> locations)
    {
        Path path = new(identifier, parentDepartment);
        short depth = GetDepth(parentDepartment);
        
        Id = Guid.NewGuid();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Name = name;
        Identifier = identifier;
        ParentDepartment = parentDepartment;
        Path = path;
        Depth = depth;
        _locations = [.. locations]; 
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

    public static Result<Department, Errors> Create(Name name, 
        Identifier identifier,
        Department? parentDepartment,
        IEnumerable<DepartmentLocation> locations)
    {
        if (!locations.Any())
        {
            return GeneralErrors.ValueIsInvalid("Department.Locations", "Department must have at least one location").ToErrors();
        }
        
        return new Department(name, identifier, parentDepartment, locations);
    }
    
    private static short GetDepth(Department? parentDepartment)
    {
        if (parentDepartment == null)
            return 1;

        return (short)(parentDepartment.Depth + 1);
    }
}