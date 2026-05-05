using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using Shared;

namespace DirectoryService.Application.Features.Positions;

public interface IPositionsRepository : IRepository
{
    Task<Result<Guid, Errors>> AddAndSave(Position position, CancellationToken cancellationToken);   
}