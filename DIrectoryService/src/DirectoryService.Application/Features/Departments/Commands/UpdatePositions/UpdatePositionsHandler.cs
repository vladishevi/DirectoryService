using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Domain.Departments;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Core.Validation;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Commands;

public class UpdatePositionsHandler(
    IDepartmentsRepository departmentsRepository,
    IPositionsRepository positionsRepository,
    IValidator<UpdatePositionsCommand> validator,
    ITransactionManager transactionManager,
    ILogger<UpdatePositionsHandler> logger)
    : ICommandHandler<Guid, UpdatePositionsCommand>
{
    public async Task<Result<Guid, Errors>> Handle(UpdatePositionsCommand command, CancellationToken cancellationToken)
    {
        ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            return result.ToErrors();
        }
        
        Result<Department, Errors> getDepartmentResult =
            await departmentsRepository.GetByIdWithPositions(command.DepartmentId, cancellationToken);
        if (getDepartmentResult.IsFailure)
        {
            logger.LogError("Error getting department with id {id} while updating positions", command.DepartmentId);
            return getDepartmentResult.Error;
        }
        
        Department department = getDepartmentResult.Value;
        Result<bool, Errors> positionsExistResult = await positionsRepository.AllExist(command.Request.PositionIds, active: true, cancellationToken);
        if (positionsExistResult.IsFailure)
        {
            logger.LogError("Error checking if positions exist while updating positions of department with id {id}", command.DepartmentId);
            return positionsExistResult.Error;
        }
        if (!positionsExistResult.Value)
        {
            logger.LogError("Position does not exist while updating positions of department with id {id}", command.DepartmentId);
            return GeneralErrors.NotFoundOrInactive("Position not found or inactive").ToErrors();       
        }

        department.UpdatePositions(command.Request.PositionIds);
        
        UnitResult<Errors> saveChangesResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            logger.LogError("Error saving changes to department with id {id}", command.DepartmentId);
            return saveChangesResult.Error;
        }
        
        return department.Id;
    }
}
