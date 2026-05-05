using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Features.Departments;

public class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCoreDepartmentsRepository> _logger;

    public EfCoreDepartmentsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<EfCoreDepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> AddAndSave(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            _dbContext.SaveChangesAsync(cancellationToken);
            return department.Id;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException pgException)
        {
            if (pgException.SqlState != PostgresErrorCodes.UniqueViolation)
            {
                _logger.LogError("Database update error while creating new department with name {name}",
                    department.Name.Value);
                return DepartmentsErrors.DatabaseError().ToErrors();
            }

            if (pgException.ConstraintName.Contains(Constants.Indexes.DEPARTMENT_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                return DepartmentsErrors.NameConflict(department.Name.Value).ToErrors();
            }

            if (pgException.ConstraintName.Contains(Constants.Indexes.DEPARTMENT_IDENTIFIER,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return DepartmentsErrors.IdentifierConflict(department.Identifier.Value).ToErrors();
            }

            return Error.Conflict().ToErrors();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while creating new department with name {name}",
                department.Name.Value);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while creating new department with name {name}",
                department.Name.Value);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<Department, Errors>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Department? department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (department != null)
            {
                return department;
            }

            _logger.LogWarning("Department not found with id {id}", id);
            return GeneralErrors.NotFound("Department not found", id).ToErrors();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while getting department with id {id}", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while getting department with id {id}", id);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }
    }
    
    public async Task<Result<Department, Errors>> GetByIdWithLocations(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Department? department = await _dbContext.Departments
                .Include(d => d.Locations)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            
            if (department != null)
            {
                return department;
            }

            _logger.LogWarning("Department not found with id {id}", id);
            return GeneralErrors.NotFound("Department not found", id).ToErrors();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while getting department with id {id}", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while getting department with id {id}", id);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<bool, Errors>> Exists(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Departments.AnyAsync(d => d.Id == id, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while checking if department with id {id} exists", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while checking if department with id {id} exists", id);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }
    }
}
