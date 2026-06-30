using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Commands;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Request.Name)
            .MustBeValueObject(Name.Create);
        
        RuleFor(d => d.Request.Identifier)
            .MustBeValueObject(Identifier.Create);

        RuleFor(d => d.Request.LocationIds)
            .NotEmpty().WithError(GeneralErrors.ValueIsInvalid("At least one location must be provided"));

        RuleFor(d => d.Request.LocationIds)
            .Must(l => l.Distinct().Count() == l.Count())
            .WithError(GeneralErrors.ValueIsInvalid("Locations must be unique"));
    }
}