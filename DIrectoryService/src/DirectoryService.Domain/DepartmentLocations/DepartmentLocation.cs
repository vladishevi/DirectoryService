namespace DirectoryService.Domain.DepartmentLocations;

public class DepartmentLocation
{
    public DepartmentLocation(Guid departementId, Guid locationId)
    {
        Id = Guid.NewGuid();
        DepartementId = departementId;
        LocationId = locationId;
    }
    
    public Guid Id { get; private set; }
    public Guid DepartementId { get; private set; }
    public Guid LocationId { get; private set; }
}