using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{

    public CreateLocationValidator()
    {
        RuleFor(l => l.Name)
            .NotEmpty().WithMessage("Name must not be empty")
            .Length(Name.MIN_LENGHT, Name.MAX_LENGHT)
            .WithMessage($"Name must be between {Name.MIN_LENGHT} and {Name.MAX_LENGHT} characters");

        RuleFor(l => l.City)
            .NotEmpty().WithMessage("City must not be empty");

        RuleFor(l => l.Street)
            .NotEmpty().WithMessage("Street must not be empty");

        RuleFor(l => l.Building)
            .GreaterThan(0).WithMessage("Building number must be greater than 0");

        RuleFor(l => l.Postcode)
            .NotEmpty().WithMessage("Postcode must not be empty");

        RuleFor(l => l.Timezone)
            .NotEmpty().WithMessage("Timezone must not be empty")
            .Must(timezone => TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out TimeZoneInfo? _))
            .WithMessage("Timezone isn't valid");
    }
}
