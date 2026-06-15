using DirectoryService.Contracts.Common;

namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest
{
    public PaginationRequest Pagination { get; init; } = new();
}