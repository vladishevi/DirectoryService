using Shared;

namespace DirectoryService.Presenters;

public record Envelope
{
    private Envelope(object? result, Errors? errors)
    {
        Result = result;
        Errors = errors;
    }
    
    public Object Result { get; }
    public Errors Errors { get; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    
    public static Envelope Ok(object? result = null) => new Envelope(result, null);
    public static Envelope Error(Errors errors) => new Envelope(null, errors);
}