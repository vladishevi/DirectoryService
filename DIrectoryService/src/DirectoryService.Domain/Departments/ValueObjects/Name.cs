using CSharpFunctionalExtensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public sealed record Name
{
    public const int MAX_LENGHT = 150;
    private const int MIN_LENGHT = 3;

    private Name(string value)
    {
        Value = value;
    }
    
    public string Value { get; }
    
    public static Result<Name, Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("Department.Name", "Name can not be empty").ToErrors();
        }

        if (value.Length < MIN_LENGHT || value.Length > MAX_LENGHT)
        {
            return GeneralErrors.ValueIsInvalid("Department.Name", $"Name must be between {MIN_LENGHT} and {MAX_LENGHT} characters").ToErrors();
        }

        return new Name(value);
    }
    
}