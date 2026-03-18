using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record Name
{
    private const int MIN_LENGHT = 3;
    private const int MAX_LENGHT = 150;

    public string Value { get; }
    
    private Name(string value)
    {
        Value = value;
    }
    
    public static Result<Name, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return  "Department name cannot be empty";
        }
        
        if (value.Length is < MIN_LENGHT or > MAX_LENGHT)
        {
            return $"Department name must be between {MIN_LENGHT} and {MAX_LENGHT} characters";
        }
        
        return new Name(value);
    }
}