using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public record Timezone
{
    private Timezone(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static Result<Timezone, string> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return "Timezone cannot be empty";
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(code, out TimeZoneInfo? _))
        {
            return "Timezone isn't valid";
        }
        
        return new Timezone(code);
    }
}