using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Commands;

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

        //open transaction
        Result<ITransactionScope, Errors> beginTransactionResult = await _transactionManager.BeginAsync(ct);
        if (beginTransactionResult.IsFailure)
        {
            _logger.LogError("Error starting transaction while updating parent");
            return beginTransactionResult.Error;
        }
        
        using ITransactionScope transaction = beginTransactionResult.Value;
        
        //get department with lock and check for active
        Result<Department, Errors> getDepartmentWithLockResult = await _departmentsRepository.GetByIdWithLock(command.DepartmentId, ct);
        if (getDepartmentWithLockResult.IsFailure)
        {
            _logger.LogError("Error getting department with id {id} while updating parent", command.DepartmentId);
            transaction.Rollback(ct);
            return getDepartmentWithLockResult.Error;
        }
        Department department = getDepartmentWithLockResult.Value;
        
        //get parent with lock and check for active
        Department? parentDepartment = null;
        if (command.Request.ParentId != null)
        {
            Result<Department, Errors> getParentWithLockResult = await _departmentsRepository.GetByIdWithLock((Guid)command.Request.ParentId, ct);
            if (getParentWithLockResult.IsFailure)
            {
                _logger.LogError("Error getting parent department with id {id} while updating parent", command.Request.ParentId);
                transaction.Rollback(ct);
                return getParentWithLockResult.Error;
            }

            parentDepartment = getParentWithLockResult.Value;
        }

        //lock descendants
        UnitResult<Errors> lockDescendants = await _departmentsRepository.LockDescendants(department.Id, ct);
        if (lockDescendants.IsFailure)
        {
            _logger.LogError("Error locking subtree of department with id {id} while updating parent", department.Id);
            transaction.Rollback(ct);
            return lockDescendants.Error;
        }
        
        //check if a parent isn't a department child
        if (command.Request.ParentId != null)
        {
            Result<bool, Errors> isParentDescendantResult =
                await _departmentsRepository.IsDescendantOf((Guid)command.Request.ParentId, command.DepartmentId, ct);
            if (isParentDescendantResult.IsFailure)
            {
                _logger.LogError("Error checking if parent department with id {id} is a descendant of department with id {departmentId}",
                    command.Request.ParentId, command.DepartmentId);
                return isParentDescendantResult.Error;
            }
            if (isParentDescendantResult.Value)
            {
                _logger.LogError("Parent department with id {id} is a descendant of department with id {departmentId}",
                    command.Request.ParentId, command.DepartmentId);
                return DepartmentsErrors.HierarchyError().ToErrors();
            }
        }

        //move subtree
        UnitResult<Errors> moveSubtreeResult = await _departmentsRepository.ChangeParentTo(department.Id, parentDepartment?.Id, ct);
        if (moveSubtreeResult.IsFailure)
        {
            _logger.LogError("Error moving subtree of department with id {id} while updating parent", department.Id);
            transaction.Rollback(ct);
            return moveSubtreeResult.Error;
        }
        
        //commit transaction
        UnitResult<Errors> commitTransactionResult = await transaction.Commit(ct);
        if (commitTransactionResult.IsFailure)
        {
            _logger.LogError("Error committing transaction while updating parent");
            transaction.Rollback(ct);
            return commitTransactionResult.Error;
        }
        
        _logger.LogInformation("Parent updated for department with id {id}", department.Id);
        return department.Id;
    }
}