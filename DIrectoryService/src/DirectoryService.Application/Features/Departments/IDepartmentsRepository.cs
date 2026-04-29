using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Errors>> Add(Department department, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetById(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Department>, Errors>> GetById(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}