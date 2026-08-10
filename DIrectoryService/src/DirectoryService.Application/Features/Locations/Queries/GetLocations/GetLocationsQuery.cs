using DirectoryService.Contracts.Locations;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Locations.GetLocations;

public record GetLocationsQuery(GetLocationsRequest Request) : IQuery;