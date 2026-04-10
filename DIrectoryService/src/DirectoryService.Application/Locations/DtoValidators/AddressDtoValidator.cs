using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations.CreateLocation;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(a => a.City)
            .NotEmpty().WithMessage("City must not be empty");

        RuleFor(a => a.Street)
            .NotEmpty().WithMessage("Street must not be empty");

        RuleFor(a => a.Building)
            .GreaterThan(0).WithMessage("Building number must be greater than 0");

        RuleFor(a => a.Postcode)
            .NotEmpty().WithMessage("Postcode must not be empty");
    }
}