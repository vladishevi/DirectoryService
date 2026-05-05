using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class UpdateLocationsHandler : ICommandHandler<Guid, UpdateLocationsCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<UpdateLocationsCommand> _validator;
    private readonly ILogger<UpdateLocationsHandler> _logger;

    public UpdateLocationsHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        IValidator<UpdateLocationsCommand> validator,
        ILogger<UpdateLocationsHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Errors>> Handle(UpdateLocationsCommand command, CancellationToken cancellationToken)
    {
        //validation
        ValidationResult result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            return result.ToErrors();
        }
        
        //get department
        Result<Department, Errors> getDepartmentResult =
            await _departmentsRepository.GetByIdWithLocations(command.DepartmentId, cancellationToken);
        if (getDepartmentResult.IsFailure)
        {
            _logger.LogError("Error getting department with id {id} while updating locations", command.DepartmentId);
            return getDepartmentResult.Error;
        }
        
        Department department = getDepartmentResult.Value;
        if (!department.IsActive)
        {
            _logger.LogError("Department with {name} is inactive while updating locations}", department.Name);
            return GeneralErrors.Inactive("Department is inactive", department.Id).ToErrors();      
        }
        
        //check if locations exist
        Result<bool, Errors> locationsExistResult = await _locationsRepository.AllExist(command.Request.LocationIds, active: true, cancellationToken);
        if (locationsExistResult.IsFailure)
        {
            _logger.LogError("Error checking if locations exist while updating locations of department with id {id}", command.DepartmentId);
            return locationsExistResult.Error;
        }
        if (!locationsExistResult.Value)
        {
            _logger.LogError("Location does not exist while updating locations of department with id {id}", command.DepartmentId);
            return GeneralErrors.NotFoundOrInactive("Location not found or inactive").ToErrors();       
        }

        //update locations
        department.UpdateLocations(command.Request.LocationIds);
        
        //db save
        Result<int, Errors> saveChangesResult = await _departmentsRepository.SaveChanges(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Error saving changes to department with id {id}", command.DepartmentId);
            return saveChangesResult.Error;
        }
        
        return department.Id;
    }
}