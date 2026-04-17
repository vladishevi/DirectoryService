using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationValidator()
    {
        RuleFor(l => l.Name)
            .NotEmpty().WithMessage("Name must not be empty")
            .Length(Name.MIN_LENGHT, Name.MAX_LENGHT)
            .WithMessage($"Name must be between {Name.MIN_LENGHT} and {Name.MAX_LENGHT} characters");

        RuleFor(l => l.Address)
            .NotNull().WithMessage("Address must not be null");
        
        RuleFor(l => l.Address.City)
            .NotEmpty().WithMessage("City must not be empty");

        RuleFor(l => l.Address.Street)
            .NotEmpty().WithMessage("Street must not be empty");

        RuleFor(l => l.Address.Building)
            .GreaterThan(0).WithMessage("Building number must be greater than 0");

        RuleFor(l => l.Address.Postcode)
            .NotEmpty().WithMessage("Postcode must not be empty");
        
        RuleFor(l => l.Timezone)
            .NotEmpty().WithMessage("Timezone must not be empty")
            .Must(timezone => TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out TimeZoneInfo? _))
            .WithMessage("Timezone isn't valid");
    }
}
