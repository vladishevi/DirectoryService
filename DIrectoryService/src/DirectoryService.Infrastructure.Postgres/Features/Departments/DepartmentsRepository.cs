using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Features.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Features.Departments;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(
        DirectoryServiceDbContext dbContext,
        DbConnectionFactory dbConnectionFactory,
        ILogger<DepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Add(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            return department.Id;
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

    public async Task<Result<bool, Errors>> Exists(Guid id, bool active, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Department> query = _dbContext.Departments.Where(d => d.Id == id);
            if (active)
                query = query.Where(d => d.IsActive);

            return await query.AnyAsync(cancellationToken);
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

    public async Task<Result<bool, Errors>> IsDescendantOf(Guid descendantId, Guid ancestorId, CancellationToken ct)
    {
        try
        {
            Department descendant = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == descendantId, ct);
            Department ancestor = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == ancestorId, ct);

            if (descendant == null || ancestor == null)
            {
                return GeneralErrors.NotFound("Department or ancestor not found").ToErrors();
            }
            
            using IDbConnection connection = await _dbConnectionFactory.CreateConnection();
            const string sql = """
                               Select @descendantPath::ltree <@ @ancestorPath::ltree
                               """;
            
            var value =  await connection.QuerySingleAsync<bool>(sql, new { descendantPath = descendant.Path.Value, ancestorPath = ancestor.Path.Value });
            return value;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while checking the department hierarchy");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while checking the department hierarchy");
            return DepartmentsErrors.DatabaseError().ToErrors();
        }

        throw new InvalidOperationException();
    }
}
