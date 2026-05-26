namespace DirectoryService.Contracts.Departments;

public record UpdatePositionsRequest(IEnumerable<Guid> PositionIds);
