using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record Name
{
    public const int MAX_LENGHT = 150;
    private const int MIN_LENGHT = 3;

    private Name(string value)
    {
        Value = value;
    }
    
    public string Value { get; }
    
    public static Result<Name, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Name cannot be empty";
        }

        if (value.Length < MIN_LENGHT || value.Length > MAX_LENGHT)
        {
            return $"Name must be between {MIN_LENGHT} and {MAX_LENGHT} characters";
        }

        return new Name(value);
    }
    
}