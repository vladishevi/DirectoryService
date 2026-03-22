namespace DirectoryService.Domain.DepartmentLocations;

public class DepartmentLocation
{
    public DepartmentLocation(Guid departementId, Guid locationId)
    {
        DepartementId = departementId;
        LocationId = locationId;
    }
    
    public Guid DepartementId { get; private set; }
    public Guid LocationId { get; private set; }
}