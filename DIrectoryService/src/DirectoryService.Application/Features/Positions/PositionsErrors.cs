using System.Runtime.InteropServices.JavaScript;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Positions;

public static class PositionsErrors
{
    public static JSType.Error NameConflict(string? name = null) =>
        JSType.Error.Conflict("position.name.already.exists", name != null 
            ? $"Position with name '{name}' already exists" 
            : "Position with name already exists");

    public static JSType.Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("position.database.error", "Positions database error");
    }
}