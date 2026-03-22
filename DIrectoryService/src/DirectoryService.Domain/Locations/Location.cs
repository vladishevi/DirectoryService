using DirectoryService.Domain.Common;

namespace DirectoryService.Domain.Locations;

public class Location
{
    public Location(Name name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
}