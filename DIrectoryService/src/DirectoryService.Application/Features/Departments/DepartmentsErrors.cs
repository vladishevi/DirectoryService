using Shared;

namespace DirectoryService.Application.Features.Departments;

public static class DepartmentsErrors
{
    public static Error NameConflict(string name) =>
        Error.Conflict("department.name.already.exists", $"Department with name '{name}' already exists");

    public static Error IdentifierConflict(string identifier) =>
        Error.Conflict("department.identifier.already.exists",
            $"Department with identifier '{identifier}' already exists");

    public static Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("department.database.error", "Departments database error");
    }
}
