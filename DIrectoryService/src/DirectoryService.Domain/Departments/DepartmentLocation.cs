namespace DirectoryService.Domain.Departments;

public class DepartmentLocation
{
    // EF Core
    private DepartmentLocation() { }
    
    public DepartmentLocation(Department departement, Guid locationId)
    {
        Id = Guid.NewGuid();
        Department = departement;
        DepartmentId = departement.Id;
        LocationId = locationId;
    }
    
    public Guid Id { get; private set; }
    public Department Department { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }
}