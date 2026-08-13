using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Departments;
using DirectoryService.Domain.Positions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Core.Validation;
using Shared.Errors;
using Name = DirectoryService.Domain.Positions.Name;

namespace DirectoryService.Application.Features.Positions;

public class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreatePositionCommand> _validator;
    private readonly ILogger<CreatePositionHandler> _logger;

    public CreatePositionHandler(
        IPositionsRepository positionsRepository,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        IValidator<CreatePositionCommand> validator,
        ILogger<CreatePositionHandler> logger)
    {
        _positionsRepository = positionsRepository;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Errors>> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        //input validation
        ValidationResult result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            return result.ToErrors();
        }
        
        //create position
        Name name = Name.Create(command.Request.Name).Value;
        Description description = command.Request.Description == null ? null : Description.Create(command.Request.Description).Value;
        Position position = new(name, description);

        //check if departments exist
        foreach (Guid departmentId in command.Request.DepartmentIds)
        {
            Result<bool, Errors> departmentExistsResult = await _departmentsRepository.Exists(departmentId, cancellationToken);
            if (departmentExistsResult.IsFailure)
            {
                _logger.LogError("Error getting department with id {id} while creating position with name {name}",
                    departmentId, command.Request.Name);
                return departmentExistsResult.Error;
            }
            
            if (!departmentExistsResult.Value)
            {
                _logger.LogError("Department with id {id} does not exist while creating position with name {name}",
                    departmentId, command.Request.Name);
                return GeneralErrors.NotFound("Department not found", departmentId).ToErrors();
            }           
        }

        //add departments to position
        position.UpdateDepartments(command.Request.DepartmentIds);

        //Add position
        Result<Guid, Errors> addPositionResult = await _positionsRepository.Add(position, cancellationToken);
        if (addPositionResult.IsFailure)
        {
            _logger.LogError("Failed to create new position with name {positionName}", position.Name);
            return addPositionResult.Error;
        }
        
        //db save
        UnitResult<Errors> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Error saving position to database: {error}", saveResult.Error);
            return saveResult.Error;           
        }

        //logging
        _logger.LogInformation("New position has been created. Name: {name}, Guid: {guid}", position.Name.Value, position.Id);
        return position.Id;       
    }
}