using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments;

public static class DepartmentsErrors
{
    public static Error NameConflict(string? name = null)
    {
        return Error.Conflict("department.name.already.exists",
            name != null
                ? $"Department with name '{name}' already exists"
                : "Department with name already exists");
    }

    public static Error IdentifierConflict(string? identifier = null)
    {
        return Error.Conflict("department.identifier.already.exists",
            identifier != null
                ? $"Department with identifier '{identifier}' already exists"
                : "Department with identifier already exists");
    }

    public static Error HierarchyError()
    {
        return Error.Conflict("department.hierarchy.error");
    }

    public static Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("department.database.error", "Departments database error");
    }
}
