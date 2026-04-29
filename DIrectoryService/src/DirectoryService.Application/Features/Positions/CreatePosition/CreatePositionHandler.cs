using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Departments;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;
using Name = DirectoryService.Domain.Positions.Name;

namespace DirectoryService.Application.Features.Positions;

public class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IValidator<CreatePositionCommand> _validator;
    private readonly ILogger<CreatePositionHandler> _logger;

    public CreatePositionHandler(
        IPositionsRepository positionsRepository,
        IDepartmentsRepository departmentsRepository,
        IValidator<CreatePositionCommand> validator,
        ILogger<CreatePositionHandler> logger)
    {
        _positionsRepository = positionsRepository;
        _departmentsRepository = departmentsRepository;
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
        Description description = Description.Create(command.Request.Description).Value;
        Position position = new(name, description);

        //get departments from db
        Result<IEnumerable<Department>, Errors> getDepartmentsResult =
            await _departmentsRepository.GetById(command.Request.DepartmentIds, cancellationToken);
        if (getDepartmentsResult.IsFailure)
        {
            _logger.LogError("Error getting departments with ids {ids} while creating position with name {name}",
                command.Request.DepartmentIds, command.Request.Name);
            return getDepartmentsResult.Error;
        }

        //add position to departments
        IEnumerable<Department> departments = getDepartmentsResult.Value;
        foreach (Department department in departments)
        {
            UnitResult<Errors> addPositionsResult = department.AddPositions(command.Request.DepartmentIds);
            if (addPositionsResult.IsFailure)
            {
                _logger.LogError("Failed to add position to department with name {departmentName}", department.Name);
                return addPositionsResult.Error;
            }
        }

        //save to db
        Result<Guid, Errors> addPositionResult = await _positionsRepository.Add(position, cancellationToken);
        if (addPositionResult.IsFailure)
        {
            _logger.LogError("Failed to create new position with name {positionName}", position.Name);
            return addPositionResult.Error;
        }

        //logging
        _logger.LogInformation("New position has been created. Name: {name}, Guid: {guid}", position.Name.Value, position.Id);
        return position.Id;       
    }
}