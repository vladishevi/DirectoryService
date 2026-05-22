using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Positions;

public class DeletePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<DeletePositionHandler> logger)
    : ICommandHandler<Guid, DeletePositionCommand>
{
    public async Task<Result<Guid, Errors>> Handle(DeletePositionCommand command, CancellationToken cancellationToken)
    {
        Result<ITransactionScope, Errors> transactionResult = await transactionManager.BeginAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            logger.LogError("Error starting transaction while deleting position");
            return transactionResult.Error;
        }
        
        using ITransactionScope transaction = transactionResult.Value;
        
        Result<Position, Errors> getPositionResult = await positionsRepository.GetById(command.Id, cancellationToken);
        if (getPositionResult.IsFailure)
        {
            logger.LogError("Error getting position with id {id} while deleting position", command.Id);
            transaction.Rollback(cancellationToken);
            return getPositionResult.Error;
        }
        
        Result<Guid, Errors> deleteResult = await positionsRepository.Delete(getPositionResult.Value);
        if (deleteResult.IsFailure)
        {
            logger.LogError("Error deleting position with id {id}", command.Id);
            transaction.Rollback(cancellationToken);
            return deleteResult.Error;
        }
        
        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            logger.LogError("Error saving changes while deleting position");
            transaction.Rollback(cancellationToken);
            return saveResult.Error;      
        }
        
        var commitResult = await transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
        {
            logger.LogError("Error committing transaction while deleting position");
            transaction.Rollback(cancellationToken);
            return commitResult.Error;       
        }
        
        logger.LogInformation("Position with id {id} has been deleted", command.Id);
        return deleteResult.Value;       
    }
}