namespace Shared;

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
}