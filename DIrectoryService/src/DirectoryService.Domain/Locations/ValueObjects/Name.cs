using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations;

public record Name
{
    public const int MAX_LENGHT = 120;
    public const int MIN_LENGHT = 3;

    private Name(string value)
    {
        Value = value;
    }
    
    public string Value { get; }
    
    public static Result<Name, Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Errors errors = Error.Validation("Name cannot be empty", invalidField: "Location.Name");
            return errors;
        }

        if (value.Length < MIN_LENGHT || value.Length > MAX_LENGHT)
        {
            Errors errors = Error.Validation($"Name must be between {MIN_LENGHT} and {MAX_LENGHT} characters", invalidField: "Location.Name");
            return errors;
        }

        return new Name(value);
    }
    
}