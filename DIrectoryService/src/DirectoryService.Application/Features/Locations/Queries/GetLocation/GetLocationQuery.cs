using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Locations;

public record GetLocationQuery(Guid LocationId) : IQuery;
