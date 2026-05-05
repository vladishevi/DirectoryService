using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Departments;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public interface IDepartmentsRepository : IRepository
{
    Task<Result<Guid, Errors>> AddAndSave(Department department, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetById(Guid id, CancellationToken cancellationToken);
    Task<Result<Department, Errors>> GetByIdWithLocations(Guid id, CancellationToken cancellationToken);
    Task<Result<bool, Errors>> Exists(Guid id, CancellationToken cancellationToken);
    Task<Result<int, Errors>> SaveChanges(CancellationToken cancellationToken);
}