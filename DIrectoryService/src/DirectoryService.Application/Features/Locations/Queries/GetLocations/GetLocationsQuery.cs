using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Features.Locations.GetLocations;

public record GetLocationsQuery(GetLocationsRequest Request) : IQuery;