using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public interface IDepartmentsRepository : IRepository
{
    Task<Result<Guid, Errors>> Add(Department department, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetById(Guid id, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetByIdWithLocations(Guid id, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetByIdWithPositions(Guid id, CancellationToken cancellationToken);
    Task<Result<bool, Errors>> Exists(Guid id, bool active, CancellationToken cancellationToken);
    Task<Result<bool, Errors>> IsDescendantOf(Guid descendantId, Guid ancestorId, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetByIdWithLock(Guid id, CancellationToken cancellationToken);
    Result<Guid, Errors> Delete(Department department);
    Task<UnitResult<Errors>> LockDescendants(Guid departmentId, CancellationToken ct);
    Task<UnitResult<Errors>> ChangeParentTo(Guid departmentId, Guid? newParentId, CancellationToken ct);
}
