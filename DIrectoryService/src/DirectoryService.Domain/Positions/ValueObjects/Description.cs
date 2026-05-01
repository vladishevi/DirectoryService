using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Positions;

public sealed record Description
{
    public const int MAX_LENGTH = 1000;
    
    public string Value { get;}

    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description, Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("position.description", "Description cannot be null").ToErrors();
        }
        
        if (value.Length > MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid("position.description", "Description name must be no longer than {MAX_LENGTH}")
                .ToErrors();
        }

        return new Description(value);
    }
}