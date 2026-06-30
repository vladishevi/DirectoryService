using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Positions;

public static class PositionsErrors
{
    public static Error NameConflict(string? name = null) =>
        Error.Conflict("position.name.already.exists", name != null 
            ? $"Position with name '{name}' already exists" 
            : "Position with name already exists");

    public static Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("position.database.error", "Positions database error");
    }
}