namespace DirectoryService.Contracts.Locations;

public record GetLocationsDto(List<GetLocationsItemDto> Locations, long totalCount);