using System.Runtime.InteropServices.JavaScript;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments;

public static class DepartmentsErrors
{
    public static JSType.Error NameConflict(string? name = null)
    {
        return JSType.Error.Conflict("department.name.already.exists",
            name != null
                ? $"Department with name '{name}' already exists"
                : "Department with name already exists");
    }

    public static JSType.Error IdentifierConflict(string? identifier = null)
    {
        return JSType.Error.Conflict("department.identifier.already.exists",
            identifier != null
                ? $"Department with identifier '{identifier}' already exists"
                : "Department with identifier already exists");
    }

    public static JSType.Error HierarchyError()
    {
        return JSType.Error.Conflict("department.hierarchy.error");
    }

    public static JSType.Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("department.database.error", "Departments database error");
    }
}
