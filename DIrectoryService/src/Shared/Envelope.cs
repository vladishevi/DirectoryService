namespace Shared;

public record Envelope<TValue>
{
    private Envelope(TValue? result, Errors? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }
    
    public TValue? Result { get; }
    public Errors? Errors { get; }
    public DateTime TimeGenerated { get; }
    
    public bool IsSuccess => Errors == null;
    
    public static Envelope<TValue> Ok(TValue? result) => new(result, null);
    public static Envelope<TValue> Error(Errors errors) => new(default, errors);
}

public record Envelope
{
    private Envelope(object? result, Errors? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }
    
    public object? Result { get; }
    public Errors? Errors { get; }
    public DateTime TimeGenerated { get; }
    
    public bool IsSuccess => Errors == null;
    
    public static Envelope Ok(object? result) => new(result, null);
    public static Envelope Error(Errors errors) => new(null, errors);
}