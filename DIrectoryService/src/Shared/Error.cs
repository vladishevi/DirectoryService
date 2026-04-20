using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    [JsonConstructor]
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
    
    public static Error NotFound(string code, string message, Guid? guid) =>
        new(code, $"{message}. Record id: {guid}", ErrorType.NOT_FOUND, guid?.ToString());

    public static Error Validation(string code, string message, string? invalidField = null) =>
        new(code, message, ErrorType.VALIDATION, invalidField);

    public static Error Conflict(string? code = null, string? message = null) =>
        new(code ?? "conflict.error", message ?? "conflict error occured", ErrorType.CONFLICT);

    public static Error Failure(string code, string message) =>
        new (code, message, ErrorType.FAILURE);

    public Errors ToErrors() => new(this);
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT
}