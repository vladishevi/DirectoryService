using DirectoryService.Domain.Common;

namespace DirectoryService.Domain.Locations;

public class Location
{
    public Location(Name name, Address address, Timezone timezone)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        Timezone = timezone;
    }
    
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Address Address { get; private set; }
    public Timezone Timezone { get; private set; }
}