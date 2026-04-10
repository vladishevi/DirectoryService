using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationDtoValidator(IValidator<AddressDto> addressValidator)
    {
        RuleFor(l => l.Name)
            .NotEmpty().WithMessage("Name must not be empty")
            .Length(Name.MIN_LENGHT, Name.MAX_LENGHT)
            .WithMessage($"Name must be between {Name.MIN_LENGHT} and {Name.MAX_LENGHT} characters");

        RuleFor(l => l.Address)
            .NotNull().WithMessage("Address must not be null")
            .SetValidator(addressValidator);
        
        RuleFor(l => l.Timezone)
            .NotEmpty().WithMessage("Timezone must not be empty")
            .Must(timezone => TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out TimeZoneInfo? _))
            .WithMessage("Timezone isn't valid");
    }
}
