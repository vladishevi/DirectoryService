using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
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
        
        //create department
        Result<Name, Errors> nameResult = Name.Create(command.Request.Name);
        Result<Identifier, Errors> identifierResult = Identifier.Create(command.Request.Identifier);
        Department department  = Department.Create(nameResult.Value, identifierResult.Value, null).Value;
        
        //get locations from db
        List<Location> locations = []; 
        foreach (Guid locationId in command.Request.LocationIds)
        {
            Result<Location, Errors> locationResult = await _locationsRepository.GetById(locationId, cancellationToken);
            if (locationResult.IsFailure)
            {
                _logger.LogError("Error getting location with id {id} while creating department with name {name}",
                    locationId, command.Request.Name);
                return locationResult.Error;
            }
            locations.Add(locationResult.Value);
        }

        //create DepartmentLocations
        List<DepartmentLocation> departmentLocations = [];
        departmentLocations.AddRange(
            locations.Select(location => new DepartmentLocation(department, location.Id)));

        foreach (DepartmentLocation departmentLocation in departmentLocations)
        {
            Result<Errors> addResult = department.AddLocation(departmentLocation);
            if (addResult.IsFailure)
            {
                return GeneralErrors.Failure("Failed to add location to department").ToErrors();
            }
        }

        //db save
        Result<Guid, Errors> addDepartmentResult = await _departmentsRepository.Add(department, cancellationToken);
        if (addDepartmentResult.IsFailure)
        {
            _logger.LogError("Failed to create new department by name {departmentName}", department.Name);
        }

        //logging
        _logger.LogInformation("New department has been created. Name: {name}, Guid: {guid}", department.Name, department.Id);
        return department.Id;
    }
}