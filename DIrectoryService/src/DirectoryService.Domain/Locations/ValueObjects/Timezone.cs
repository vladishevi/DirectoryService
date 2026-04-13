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
            return GeneralErrors.ValueIsInvalid("Timezone cannot be empty", "Location.Timezone").ToErrors();
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(code, out TimeZoneInfo? _))
        {
            return GeneralErrors.ValueIsInvalid("Timezone isn't valid", "Location.Timezone").ToErrors();
        }
        
        return new Timezone(code);
    }
}