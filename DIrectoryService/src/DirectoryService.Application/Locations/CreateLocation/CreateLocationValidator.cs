using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationValidator()
    {
        RuleFor(l => l.Name)
            .MustBeValueObject(Name.Create);
        
        RuleFor(l => l.Address)
            .MustBeValueObject(addressDto => Address.Create(addressDto.City, addressDto.Street, addressDto.Building, addressDto.Postcode));
        
        RuleFor(l => l.Timezone)
            .MustBeValueObject(Timezone.Create);
    }
}
