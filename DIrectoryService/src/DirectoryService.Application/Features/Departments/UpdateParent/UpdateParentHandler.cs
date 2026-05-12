using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class UpdateParentHandler : ICommandHandler<Guid, UpdateParentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<UpdateParentCommand> _validator;
    private readonly ILogger<UpdateParentHandler> _logger;

    public UpdateParentHandler(
        IDepartmentsRepository departmentsRepository, 
        ITransactionManager transactionManager,
        IValidator<UpdateParentCommand> validator, 
        ILogger<UpdateParentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Errors>> Handle(UpdateParentCommand command, CancellationToken ct)
    {
        //validate command
        ValidationResult validationResult = await _validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        //check if department exists and active
        Result<bool, Errors> departmentExistsResult =  await _departmentsRepository.Exists(command.DepartmentId, active: true, ct);
        if (departmentExistsResult.IsFailure)
        {
            _logger.LogError("Error getting department with id {id} while updating parent", command.DepartmentId);
            return departmentExistsResult.Error;
        }
        if (!departmentExistsResult.Value)
        {
            _logger.LogError("Department with name {name} does not exist while updating parent", command.DepartmentId);
            return GeneralErrors.NotFound("Department not found", command.DepartmentId).ToErrors();
        }

        //check if parent exists and active
        Result<bool, Errors> parentExistsResult =  await _departmentsRepository.Exists(command.Request.ParentId, active: true, ct);
        if (parentExistsResult.IsFailure)
        {
            _logger.LogError("Error getting parent department with id {id} while updating parent", command.Request.ParentId);
            return parentExistsResult.Error;
        }

        if (!parentExistsResult.Value)
        {
            _logger.LogError("Parent department with name {name} does not exist while updating parent", command.Request.ParentId);
            return GeneralErrors.NotFound("Parent department not found", command.Request.ParentId).ToErrors();
        }

        //check if a parent isn't a department child
        throw new NotImplementedException();
    }
}