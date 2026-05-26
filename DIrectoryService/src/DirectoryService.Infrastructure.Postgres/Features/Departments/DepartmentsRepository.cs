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
            Department? department = await _dbContext.Departments.FromSqlInterpolated($"""
                                                                                           SELECT *
                                                                                           FROM departments d
                                                                                           WHERE d.id = {id}
                                                                                           FOR UPDATE
                                                                                           """).SingleOrDefaultAsync(ct);
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

    public Result<Guid, Errors> Delete(Department department)
    {
        try
        {
            _dbContext.Remove(department);
            return department.Id;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while deleting department with id {id}", department.Id);
            return DepartmentsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<UnitResult<Errors>> LockDescendants(Guid departmentId, CancellationToken ct)
    {
        try
        {
            Department? department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken: ct);
            if (department == null)
            {
                return GeneralErrors.NotFound("Department not found", departmentId).ToErrors();
            }
            
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                           SELECT id
                                                           FROM departments
                                                           WHERE 
                                                               path <@ {department.Path.Value}::ltree
                                                               AND id <> {departmentId}
                                                           FOR UPDATE
                                                           """, cancellationToken: ct);
            return UnitResult.Success<Errors>();           
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while locking descendants of department with id {id}", departmentId);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while locking descendants of department with id {id}", departmentId);           
            return DepartmentsErrors.DatabaseError().ToErrors();
        }       
    }

    public async Task<UnitResult<Errors>> ChangeParentTo(Guid departmentId, Guid? newParentId, CancellationToken ct)
    {
        try
        {
            Department? department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken: ct);
            if (department == null)
            {
                return GeneralErrors.NotFound("Department not found").ToErrors();
            }

            Department? newParent = null;
            if (newParentId != null)
            {
                newParent = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == newParentId, cancellationToken: ct);
            }

            //change parent id
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                   UPDATE departments d
                                                                   SET parent_department_id = {newParentId}
                                                                   WHERE d.id = {departmentId}
                                                                   """, ct);
            
            //change path and depth
            if (newParent == null)
            {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                       UPDATE departments
                                                                       SET 
                                                                           path = subpath(path, nlevel({department.Path.Value}::ltree) - 1),
                                                                           depth = nlevel(path) - nlevel({department.Path.Value}::ltree)
                                                                       WHERE path <@ {department.Path.Value}::ltree
                                                                       """, ct);
            }
            else
            {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                       UPDATE departments
                                                                       SET 
                                                                           path = {newParent.Path.Value}::ltree 
                                                                           || subpath(path, nlevel({department.Path.Value}::ltree) - 1),
                                                                           depth = nlevel({newParent.Path.Value}::ltree) 
                                                                           + nlevel(path) - nlevel({department.Path.Value}::ltree)
                                                                       WHERE path <@ {department.Path.Value}::ltree
                                                                       """, ct);
                
            }
            
            return UnitResult.Success<Errors>();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while changing parent of a department with id {id}", departmentId);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while changing parent of a department with id {id}", departmentId);          
            return DepartmentsErrors.DatabaseError().ToErrors();
        }       
    }
}
