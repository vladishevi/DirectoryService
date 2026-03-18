using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record Identifier
{
    private const int MIN_LENGHT = 3;
    private const int MAX_LENGHT = 150;

    public string Value { get; }
    
    private Identifier(string value)
    {
        Value = value;
    }
    
    public static Result<Identifier, string> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return  "Department identifier cannot be empty";
        }
        
        if (value.Length is < MIN_LENGHT or > MAX_LENGHT)
        {
            return $"Department identifier must be between {MIN_LENGHT} and {MAX_LENGHT} characters";
        }

        bool isLatin = Regex.IsMatch(value, "^[A-Za-z]+$");
        if (!isLatin)
        {
            return "Department identifier must contain latin characters only";
        }
        
        return new Identifier(value);
    }
}