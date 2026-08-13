using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Commands;

public class DeleteLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteLocationHandler> logger)
    : ICommandHandler<Guid, DeleteLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(DeleteLocationCommand command, CancellationToken cancellationToken)
    {
        Result<Department, Errors> getDepartmentResult = await departmentsRepository.GetByIdWithLocations(command.DepartmentId, cancellationToken);
        if (getDepartmentResult.IsFailure)
        {
            logger.LogError("Error getting department with id {id} while updating locations", command.DepartmentId);
            return getDepartmentResult.Error;
        }
        
        Department department = getDepartmentResult.Value;
        department.RemoveLocation(command.LocationId);
        
        transactionManager.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Location with id {id} has been deleted from department with id {departmentId}", command.LocationId, command.DepartmentId);
        return command.DepartmentId;
    }
}