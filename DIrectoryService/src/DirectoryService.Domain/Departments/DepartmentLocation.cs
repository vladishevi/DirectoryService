namespace DirectoryService.Domain.Departments;

public sealed class DepartmentLocation
{
    // EF Core
    private DepartmentLocation() { }
    
    public DepartmentLocation(Department departement, Guid locationId)
    {
        Department = departement;
        DepartmentId = departement.Id;
        LocationId = locationId;
    }
    
    public Guid Id { get; private set; }
    public Department Department { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }
}