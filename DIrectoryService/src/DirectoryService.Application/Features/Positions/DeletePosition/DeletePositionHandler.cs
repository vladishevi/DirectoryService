using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Errors;

namespace DirectoryService.Application.Features.Positions;

public class DeletePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<DeletePositionHandler> logger)
    : ICommandHandler<Guid, DeletePositionCommand>
{
    public async Task<Result<Guid, Errors>> Handle(DeletePositionCommand command, CancellationToken cancellationToken)
    {
        Result<Position, Errors> getPositionResult = await positionsRepository.GetById(command.Id, cancellationToken);
        if (getPositionResult.IsFailure)
        {
            logger.LogError("Error getting position with id {id} while soft deleting position", command.Id);
            return getPositionResult.Error;
        }

        var position = getPositionResult.Value;
        position.SoftDelete();
        
        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            logger.LogError("Error saving changes while soft deleting position");
            return saveResult.Error;      
        }
        
        logger.LogInformation("Position with id {id} has been soft deleted", command.Id);
        return position.Id;       
    }
}
