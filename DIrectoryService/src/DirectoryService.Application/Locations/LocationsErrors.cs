using Shared;

namespace DirectoryService.Application.Locations;

public static class LocationsErrors
{
    public static Error AlreadyExists(string name) =>
        Error.Conflict("location.already.exists", $"Location with name {name} already exists");
}