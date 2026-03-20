using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public record Description
{
    private const int MAX_LENGTH = 1000;
    
    public string Value { get; private set; }

    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description, string> Create(string value)
    {
        if (value.Length > MAX_LENGTH)
        {
            return $"Description name must be no longer than {MAX_LENGTH}";
        }

        return new Description(value);
    }
}