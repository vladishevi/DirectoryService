using Shared.Errors;

namespace DirectoryService.Application.Features.Locations;

public static class LocationsErrors
{
    public static Error NameConflict(string? name = null) =>
        Error.Conflict("location.name.already.exists",
            name != null 
                ? $"Location with name '{name}' already exists" 
                : "Location with name already exists");
    
    public static Error AddressConflict(string? address = null) =>
        Error.Conflict("location.address.already.exists", address != null 
            ? $"Location with address '{address}' already exists" 
            : "Location with address already exists"); 

    public static Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("location.database.error", "Locations database error");
    }
}