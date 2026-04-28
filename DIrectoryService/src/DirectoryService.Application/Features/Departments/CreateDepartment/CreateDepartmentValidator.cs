using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Request.Name)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("Name is required"))
            .MustBeValueObject(Name.Create);
        
        RuleFor(d => d.Request.Identifier)
            .NotEmpty().WithError(GeneralErrors.ValueIsInvalid("Identifier is required"))
            .MustBeValueObject(Identifier.Create);

        RuleFor(d => d.Request.LocationIds)
            .NotEmpty().WithError(GeneralErrors.ValueIsInvalid("Locations are required"))
            .WithError(GeneralErrors.ValueIsInvalid("At least one location must be provided"));

        RuleFor(d => d.Request.LocationIds)
            .Must(l => l.Distinct().Count() == l.Count())
            .WithError(GeneralErrors.ValueIsInvalid("Locations must be unique"));
    }
}