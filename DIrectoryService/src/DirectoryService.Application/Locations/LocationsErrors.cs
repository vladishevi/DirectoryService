using Shared;

namespace DirectoryService.Application.Locations;

public static class LocationsErrors
{
    public static Error NameConflict(string name) =>
        Error.Conflict("location.name.already.exists", $"Location with name '{name}' already exists");
    
    public static Error AddressConflict(string address) =>
        Error.Conflict("location.address.already.exists", $"Location with address '{address}' already exists"); 

    public static Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("location.database.error", "Locations database error");
    }
}