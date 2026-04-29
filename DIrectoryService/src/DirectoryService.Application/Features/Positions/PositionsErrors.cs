using Shared;

namespace DirectoryService.Application.Features.Positions;

public static class PositionsErrors
{
    public static Error NameConflict(string name) =>
        Error.Conflict("position.name.already.exists", $"Position with name '{name}' already exists");

    public static Error DatabaseError()
    {
        return GeneralErrors.DatabaseError("position.database.error", "Positions database error");
    }
}