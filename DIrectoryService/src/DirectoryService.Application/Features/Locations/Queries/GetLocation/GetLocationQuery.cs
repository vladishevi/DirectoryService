using DirectoryService.Application.Abstractions;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Locations.GetLocation;

public record GetLocationQuery(Guid LocationId) : IQuery;
