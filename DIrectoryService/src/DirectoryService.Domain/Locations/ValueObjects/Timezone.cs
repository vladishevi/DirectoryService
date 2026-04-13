using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations;

public record Timezone
{
    private Timezone(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static Result<Timezone, Errors> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new Errors(Error.Validation("Timezone cannot be empty", invalidField: "Location.Timezone"));
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(code, out TimeZoneInfo? _))
        {
            return new Errors(Error.Validation("Timezone isn't valid", invalidField: "Location.Timezone"));
        }
        
        return new Timezone(code);
    }
}