namespace DirectoryService.Application.Features.Departments;

public record UpdateLocationsRequest(Guid DepartmentId, IEnumerable<Guid> LocationIds);