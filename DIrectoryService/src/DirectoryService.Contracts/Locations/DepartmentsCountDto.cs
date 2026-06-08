namespace DirectoryService.Contracts.Locations;

public record DepartmentsCountDto(Guid LocationId, string LocationName, int DepartmentsCount);