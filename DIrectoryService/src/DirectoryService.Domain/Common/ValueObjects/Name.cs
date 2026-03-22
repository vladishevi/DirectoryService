using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Common;

public record Name
{
    public record Options
    {
        public int? MinLenght;
        public int? MaxLenght;
        public IEnumerable<string>? CreatedNames;

        public Options(int? MinLenght = null, int? MaxLenght = null, IEnumerable<string>? CreatedNames = null)
        {
            this.MinLenght = MinLenght;
            this.MaxLenght = MaxLenght;
            this.CreatedNames = CreatedNames;
        }
    }
    
    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Name, string> Create(string value, Options? options)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "The name cannot be empty";
        }

        if (options == null)
        {
            return new Name(value);
        }

        if (options.MinLenght != null && value.Length < options.MinLenght)
        {
            return $"The name must be no shorter than {options.MinLenght} characters";
        }
        
        if (options.MaxLenght != null && value.Length > options.MaxLenght)
        {
            return $"The name must be no longer than {options.MaxLenght} characters";
        }
        
        if (options.CreatedNames != null && options.CreatedNames.Contains(value))
        {
            return "The name must be unique";
        }

        return new Name(value);
    }
}