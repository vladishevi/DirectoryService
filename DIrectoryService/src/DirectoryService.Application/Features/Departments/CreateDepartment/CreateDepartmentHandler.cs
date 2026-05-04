using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;
using Name = DirectoryService.Domain.Departments.Name;

namespace DirectoryService.Application.Features.Departments;

public class CreateDepartmentHandler : ICommandHandler<Guid,CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentCommand> _validator;

    private readonly ILogger<CreateDepartmentHandler> _logger;

    public CreateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        IValidator<CreateDepartmentCommand> validator,
        ILogger<CreateDepartmentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid,Errors>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        //input validation
        ValidationResult result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            return result.ToErrors();
        }

        Department? parentDepartment = null;
        //get the parent department from db
        if (command.Request.ParentId != null)
        {
            Result<Department, Errors> getParentDepartmentResult = await _departmentsRepository.GetById(command.Request.ParentId.Value, cancellationToken);
            if (getParentDepartmentResult.IsFailure)
            {
                _logger.LogError("Error getting parent department with id {id} while creating department with name {name}", command.Request.ParentId, command.Request.Name);
                return getParentDepartmentResult.Error;
            }
            
            parentDepartment = getParentDepartmentResult.Value;
        }
        
        //create department
        Result<Name, Errors> nameResult = Name.Create(command.Request.Name);
        Result<Identifier, Errors> identifierResult = Identifier.Create(command.Request.Identifier);
        Department department  = Department.Create(nameResult.Value, identifierResult.Value, parentDepartment).Value;
        
        //check if locations exist
        Result<bool, Errors> locationsExistResult =
            await _locationsRepository.AllExist(command.Request.LocationIds, cancellationToken);
        if (locationsExistResult.IsFailure)
        {
            _logger.LogError("Error checking if locations exist while creating department with name {name}",
                command.Request.Name);
            return locationsExistResult.Error;
        }
        if (!locationsExistResult.Value)
        {
            _logger.LogError("Location does not exist while creating department with name {name}", command.Request.Name);
            return GeneralErrors.NotFound("Location not found").ToErrors();
        }

        //add locations to department
        department.UpdateLocations(command.Request.LocationIds);

        //db save
        Result<Guid, Errors> addDepartmentResult = await _departmentsRepository.Add(department, cancellationToken);
        if (addDepartmentResult.IsFailure)
        {
            _logger.LogError("Failed to create new department by name {departmentName}", department.Name);
            return addDepartmentResult.Error;
        }

        //logging
        _logger.LogInformation("New department has been created. Name: {name}, Guid: {guid}", department.Name.Value, department.Id);
        return department.Id;
    }
}