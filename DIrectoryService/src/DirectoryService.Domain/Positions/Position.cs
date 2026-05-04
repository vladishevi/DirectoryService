using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared;

namespace DirectoryService.Domain.Positions;

public sealed class Position
{
    // EF Core
    private Position() { }
    
    public Position(Name name, Description? description = null)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
        Name = name;
        Description = description;
    }
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime UpdatedAt  { get; private set; }
    
    public IReadOnlyCollection<DepartmentPosition> Departments => _departments;
    
    private readonly List<DepartmentPosition> _departments = [];

    public UnitResult<Errors> AddDepartments(IEnumerable<Guid> departmentsIds)
    {
        if (_departments.Any(department => departmentsIds.Contains(department.Id)))
        {
            return GeneralErrors.Failure($"Position {Name} already assign to the department").ToErrors();
        }
        
        _departments.AddRange(departmentsIds.Select(departmentId => new DepartmentPosition(departmentId, Id)));
        return UnitResult.Success<Errors>();
    }
}