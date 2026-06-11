using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using FluentValidation;

namespace DirectoryService.Application.Features.Locations.Commands;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(l => l.CreateLocationRequest.Name)
            .MustBeValueObject(Name.Create);
        
        RuleFor(l => l.CreateLocationRequest.Address)
            .MustBeValueObject(addressDto => Address.Create(addressDto.City, addressDto.Street, addressDto.Building, addressDto.Postcode));
        
        RuleFor(l => l.CreateLocationRequest.Timezone)
            .MustBeValueObject(Timezone.Create);
    }
}
