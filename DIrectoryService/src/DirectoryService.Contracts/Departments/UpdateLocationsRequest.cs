namespace DirectoryService.Application.Features.Departments;

public record UpdateLocationsRequest(IEnumerable<Guid> LocationIds);