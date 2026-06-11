using DirectoryService.Contracts.Common;

namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsRequest
{
    public string? Search { get; init; }
    public PaginationRequest Pagination { get; init; } = new();
    public string SortBy { get; init; } = "Name";
    public string SortDir { get; init; } = "Asc";
}
