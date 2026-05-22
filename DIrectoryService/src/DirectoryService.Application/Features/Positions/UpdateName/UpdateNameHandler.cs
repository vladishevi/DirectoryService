using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Positions;

public class UpdateNameHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    IValidator<UpdateNameCommand> validator,
    ILogger<UpdateNameHandler> logger)
    : ICommandHandler<Guid, UpdateNameCommand>
{
    public async Task<Result<Guid, Errors>> Handle(UpdateNameCommand command, CancellationToken ct)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        //get position from repo
        Result<Position, Errors> getPositionResult = await positionsRepository.GetById(command.Id, ct);
        if (getPositionResult.IsFailure)
        {
            logger.LogError("Error getting position with id {id} while updating name", command.Id);
            return getPositionResult.Error;
        }

        Position position = getPositionResult.Value;
        
        //check if the position is active
        if (!position.IsActive)
        {
            return GeneralErrors.Inactive("Position is inactive", position.Id).ToErrors();
        }
        
        //rename
        Result<Name, Errors> newNameResult = Name.Create(command.Request.Name);
        if (newNameResult.IsFailure)
        {
            return newNameResult.Error;
        }
        
        position.UpdateName(newNameResult.Value);

        //save changes
        var saveChangesResult = await transactionManager.SaveChangesAsync(ct);
        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error;
        }
        
        //log
        logger.LogInformation("Position name updated with id: {id}", position.Id);
        return position.Id;       
    }
}