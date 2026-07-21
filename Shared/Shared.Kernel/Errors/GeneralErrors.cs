namespace Shared.Errors;

public static class GeneralErrors
{
    public static Error Failure(string? message = null)
    {
        return Error.Failure("server.failure", message ?? "Something went wrong");
    }
    
    public static Error ValueIsInvalid(string valueName, string? message = null)
    {
        return Error.Validation("value.invalid", message ?? $"{valueName} is invalid", valueName);
    }

    public static Error NotFound(string? message = null, Guid? guid = null)
    {
        return Error.NotFound("not.found", message ?? "Not found", guid);
    }
    
    public static Error Inactive(string? message = null, Guid? guid = null)
    {
        return Error.NotFound("inactive", message ?? "Inactive", guid);
    }
    
    public static Error NotFoundOrInactive(string? message = null, Guid? guid = null)
    {
        return Error.NotFound("not.found.or.inactive", message ?? "Not found or inactive", guid);
    }
    
    public static Error DatabaseError(string? code = null, string? message = null)
    {
        return Error.Failure(code ?? "database.error", message ?? "Something went wrong with the database");
    }
    
    public static Error OperationCancelled(string? code = null, string? message = null)
    {
        return Error.Failure(code ?? "operation.cancelled", message ?? "Operation cancelled");
    }
    
    public static Error Dublicate(string? code = null, string? message = null)
    {
        return Error.Failure(code ?? "dublicate.error", message ?? "Dublicate error occured");
    }
}