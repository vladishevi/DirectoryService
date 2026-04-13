namespace Shared;

public record Error
{
    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }
    
    public static Error NotFound(string message, Guid? guid, string? code = null) => 
        new(code ?? "record.not.found", message, ErrorType.NOT_FOUND, guid?.ToString());
    
    public static Error Validation(string message, string? code = null, string? invalidField = null) =>
        new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);
    
    public static Error Conflict(string message, Guid? guid, string? code = null) => 
        new(code ?? "value.is.conflict", message, ErrorType.CONFLICT, guid?.ToString());
    
    public static Error Failure(string message, string? code = null) =>
        new (code ?? "failure", message, ErrorType.FAILURE);
}

public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT
}