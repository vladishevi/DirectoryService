using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Locations.GetLocation;

public record GetLocationQuery(Guid LocationId) : IQuery;
