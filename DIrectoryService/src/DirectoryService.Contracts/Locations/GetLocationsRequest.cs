using DirectoryService.Contracts.Common;

namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest
{
    public string? Search { get; init; }
    public string SortBy { get; init; } = "Name";
    public string SortDir { get; init; } = "Asc";
    public PaginationRequest Pagination { get; init; } = new();
    public int MinDepartmentCount { get; set; }
}