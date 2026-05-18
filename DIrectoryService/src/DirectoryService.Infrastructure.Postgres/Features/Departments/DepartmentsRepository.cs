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
            Department? descendant = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == descendantId, cancellationToken: ct);
            Department? ancestor = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == ancestorId, cancellationToken: ct);
            
            if (descendant == null || ancestor == null)
            {
                return GeneralErrors.NotFound("Department not found").ToErrors();
            }
            
            const string sql = """
                               SELECT @descendantPath::ltree <@ @ancestorPath::ltree
                               """;
            
            using IDbConnection connection = await _dbConnectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<bool>(new CommandDefinition(
                sql,
                new { descendantPath = descendant.Path.Value, ancestorPath = ancestor.Path.Value },
                cancellationToken: ct));
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
    }

    public async Task<Result<Department, Errors>> GetByIdWithLock(Guid id, CancellationToken ct)
    {
        try
        {
            Department? department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken: ct);
            if (department != null)
            {
                return department;
            }
            
            _logger.LogWarning("Department not found with id {id}", id);
            return GeneralErrors.NotFound("Department not found", id).ToErrors();           
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while locking department with id {id}", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while locking department with id {id}", id);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<UnitResult<Errors>> LockSubtree(string rootPath, CancellationToken ct)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                           SELECT id
                                                           FROM departments
                                                           WHERE path <@ {rootPath}::ltree
                                                           FOR UPDATE
                                                           """, cancellationToken: ct);
            return UnitResult.Success<Errors>();           
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while locking subtree with root path {rootPath}", rootPath);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while locking subtree with root path {rootPath}", rootPath);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }       
    }

    public async Task<UnitResult<Errors>> MoveSubtree(string oldRootPath, string newRootPath, CancellationToken ct)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                   UPDATE departments
                                                                   SET path = 
                                                                   {newRootPath}::ltree || subpath(path, nlevel({oldRootPath}::ltree) - 1)
                                                                   WHERE path <@ {oldRootPath}::ltree
                                                                   """, ct);
            
            return UnitResult.Success<Errors>();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while moving subtree with root path {rootPath}", oldRootPath);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while moving subtree with root path {rootPath}", oldRootPath);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }       
    }
}
