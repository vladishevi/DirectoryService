using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Location> LocationsRead { get; }
    IQueryable<Department> DepartmentsRead { get; }
}