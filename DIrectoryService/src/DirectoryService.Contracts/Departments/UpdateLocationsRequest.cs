namespace DirectoryService.Contracts.Departments;

public record UpdateLocationsRequest(IEnumerable<Guid> LocationIds);