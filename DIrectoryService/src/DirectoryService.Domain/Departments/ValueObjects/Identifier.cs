using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public sealed record Identifier
{
    public const int MAX_LENGHT = 150;
    private const int MIN_LENGHT = 3;

    public string Value { get; }
    
    private Identifier(string value)
    {
        Value = value;
    }
    
    public static Result<Identifier, Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("Department.Identifier", "Department identifier cannot be empty").ToErrors();
        }
        
        if (value.Length is < MIN_LENGHT or > MAX_LENGHT)
        {
            return GeneralErrors.ValueIsInvalid("Department.Identifier", $"Department identifier must be between {MIN_LENGHT} and {MAX_LENGHT} characters").ToErrors();
        }

        bool isLatin = Regex.IsMatch(value, "^[A-Za-z]+$");
        if (!isLatin)
        {
            return GeneralErrors.ValueIsInvalid("Department.Identifier", "Department identifier must contain latin characters only").ToErrors();
        }
        
        return new Identifier(value.ToLower());
    }
}
