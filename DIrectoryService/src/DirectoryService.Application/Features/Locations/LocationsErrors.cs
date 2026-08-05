using System.Runtime.InteropServices.JavaScript;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Locations;

public static class LocationsErrors
{
    public static JSType.Error NameConflict(string? name = null) =>
        JSType.Error.Conflict("location.name.already.exists",
            name != null 
                ? $"Location with name '{name}' already exists" 
                : "Location with name already exists");
    
    public static JSType.Error AddressConflict(string? address = null) =>
        JSType.Error.Conflict("location.address.already.exists", address != null 
            ? $"Location with address '{address}' already exists" 
            : "Location with address already exists"); 

    public static JSType.Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("location.database.error", "Locations database error");
    }
}