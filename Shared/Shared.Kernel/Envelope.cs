using System.Text.Json.Serialization;

namespace Shared;

public record Envelope<TValue>
{
    [JsonConstructor]
    private Envelope(TValue? result, Errors.Errors? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }
    
    public TValue? Result { get; }
    public Errors.Errors? Errors { get; }
    public DateTime TimeGenerated { get; }
    
    public bool IsSuccess => Errors == null;
    
    public static Envelope<TValue> Ok(TValue? result) => new(result, null);
    public static Envelope<TValue> Error(Errors.Errors errors) => new(default, errors);
}
