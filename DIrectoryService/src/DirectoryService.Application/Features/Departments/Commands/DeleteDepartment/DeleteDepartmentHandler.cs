using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class DeleteDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteDepartmentHandler> logger)
    : ICommandHandler<Guid, DeleteDepartmentCommand>
{
    public async Task<Result<Guid, Errors>> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        Result<ITransactionScope, Errors> transactionResult = await transactionManager.BeginAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            logger.LogError("Error starting transaction while deleting department");
            return transactionResult.Error;
        }

        using ITransactionScope transaction = transactionResult.Value;

        Result<Department, Errors> getDepartmentResult = await departmentsRepository.GetById(command.Id, cancellationToken);
        if (getDepartmentResult.IsFailure)
        {
            logger.LogError("Error getting department with id {id} while deleting department", command.Id);
            await transaction.Rollback(cancellationToken);
            return getDepartmentResult.Error;
        }

        Result<Guid, Errors> deleteResult = departmentsRepository.Delete(getDepartmentResult.Value);
        if (deleteResult.IsFailure)
        {
            logger.LogError("Error deleting department with id {id}", command.Id);
            await transaction.Rollback(cancellationToken);
            return deleteResult.Error;
        }

        UnitResult<Errors> saveResult = await transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            logger.LogError("Error saving changes to database");
            await transaction.Rollback(cancellationToken);
            return saveResult.Error;
        }

        await transaction.Commit(cancellationToken);

        return deleteResult.Value;
    }
}
