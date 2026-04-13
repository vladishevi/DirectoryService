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
            return Error.Validation("Timezone cannot be empty", invalidField: "Location.Timezone").ToErrors;
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(code, out TimeZoneInfo? _))
        {
            return Error.Validation("Timezone isn't valid", invalidField: "Location.Timezone").ToErrors;
        }
        
        return new Timezone(code);
    }
}