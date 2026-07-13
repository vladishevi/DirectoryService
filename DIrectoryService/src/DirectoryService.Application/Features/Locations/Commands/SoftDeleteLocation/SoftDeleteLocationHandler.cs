using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Features.Locations.Commands.SoftDeleteLocation;

public class SoftDeleteLocationHandler(
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<SoftDeleteLocationHandler> logger) : ICommandHandler<Guid, SoftDeleteLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(SoftDeleteLocationCommand command, CancellationToken ct)
    {
        Result<Location, Errors> getLocationResult = await locationsRepository.GetById(command.Id, ct);
        if (getLocationResult.IsFailure)
        {
            logger.LogError("Error getting location with id {id} while soft deleting location", command.Id);
            return getLocationResult.Error;
        }

        var location = getLocationResult.Value;
        location.SoftDelete();

        var saveResult = await transactionManager.SaveChangesAsync(ct);
        if (saveResult.IsFailure)
        {
            logger.LogError("Error saving changes to database");
            return saveResult.Error;
        }
        
        logger.LogInformation("Location with id {id} has been soft deleted", command.Id);
        return location.Id;
    }
}