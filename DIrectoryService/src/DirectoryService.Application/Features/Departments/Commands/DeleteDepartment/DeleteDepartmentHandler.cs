using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Commands;

public class DeleteDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteDepartmentHandler> logger)
    : ICommandHandler<Guid, DeleteDepartmentCommand>
{
    public async Task<Result<Guid, Errors>> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        Result<Department, Errors> getDepartmentResult = await departmentsRepository.GetById(command.Id, cancellationToken);
        if (getDepartmentResult.IsFailure)
        {
            logger.LogError("Error getting department with id {id} while soft deleting department", command.Id);
            return getDepartmentResult.Error;
        }

        var department = getDepartmentResult.Value;
        department.SoftDelete();

        UnitResult<Errors> saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            logger.LogError("Error saving changes to database");
            return saveResult.Error;
        }

        logger.LogInformation("Department with id {id} has been soft deleted", command.Id);
        return department.Id;
    }
}
