using Shared;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public static class LocationsErrors
{
    public static Error AlreadyExists(Guid locationId) =>
        Error.Conflict("location.already.exists", $"Location with id {locationId} already exists");
}