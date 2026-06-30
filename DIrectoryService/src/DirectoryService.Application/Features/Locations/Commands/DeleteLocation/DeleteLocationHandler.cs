using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Locations.Commands;

public class DeleteLocationHandler(
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteLocationHandler> logger)
    : ICommandHandler<Guid, DeleteLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(DeleteLocationCommand command, CancellationToken cancellationToken)
    {
        Result<ITransactionScope, Errors> transactionResult = await transactionManager.BeginAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            logger.LogError("Error starting transaction while deleting location");
            return transactionResult.Error;
        }

        using ITransactionScope transaction = transactionResult.Value;

        Result<Location, Errors> getLocationResult = await locationsRepository.GetById(command.Id, cancellationToken);
        if (getLocationResult.IsFailure)
        {
            logger.LogError("Error getting location with id {id} while deleting location", command.Id);
            await transaction.Rollback(cancellationToken);
            return getLocationResult.Error;
        }

        Result<Guid, Errors> deleteResult = await locationsRepository.Delete(getLocationResult.Value);
        if (deleteResult.IsFailure)
        {
            logger.LogError("Error deleting location with id {id}", command.Id);
            await transaction.Rollback(cancellationToken);
            return deleteResult.Error;
        }

        UnitResult<Errors> saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            logger.LogError("Error saving changes to database");
            await transaction.Rollback(cancellationToken);
            return saveResult.Error;
        }

        await transaction.Commit(cancellationToken);

        return deleteResult.Value;
    }
}
