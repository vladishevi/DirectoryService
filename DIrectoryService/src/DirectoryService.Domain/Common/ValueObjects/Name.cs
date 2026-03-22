using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Common;

public record Name
{
    public record Options
    {
        public int? MinLength { get; init; }
        public int? MaxLength { get; init; }
        public IEnumerable<string>? CreatedNames { get; init; }
    }
    
    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Name, string> Create(string value, Options? options = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "The name cannot be empty";
        }

        if (options == null)
        {
            return new Name(value);
        }

        if (options.MinLength != null && value.Length < options.MinLength)
        {
            return $"The name must be no shorter than {options.MinLength} characters";
        }
        
        if (options.MaxLength != null && value.Length > options.MaxLength)
        {
            return $"The name must be no longer than {options.MaxLength} characters";
        }
        
        if (options.CreatedNames != null && options.CreatedNames.Contains(value))
        {
            return "The name must be unique";
        }

        return new Name(value);
    }
}