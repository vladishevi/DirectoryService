using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public record Name
{
    private const int MIN_LENGHT = 3;
    private const int MAX_LENGHT = 150;

    public string Value { get; }
    
    private Name(string value)
    {
        Value = value;
    }

    public static Result<Name, string> Create(string value, IEnumerable<string> createdNames)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Position name cannot be empty";
        }

        if (value.Length is < MIN_LENGHT or > MAX_LENGHT)
        {
            return $"Position name must be between {MIN_LENGHT} and {MAX_LENGHT} characters";
        }

        if (createdNames.Contains(value))
        {
            return "Position name must be unique";
        }

        return new Name(value);
    }
}