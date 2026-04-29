using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Features.Positions;

public class CreatePositionValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionValidator()
    {
        RuleFor(p => p.Request.Name)
            .MustBeValueObject(Name.Create);

        RuleFor(p => p.Request.Description)
            .MustBeValueObject(Description.Create);
        
        RuleFor(p => p.Request.DepartmentIds)
            .NotEmpty().WithError(GeneralErrors.ValueIsInvalid("At least one location must be provided"));
        
        RuleFor(p => p.Request.DepartmentIds)
            .Must(p => p.Distinct().Count() == p.Count())
            .WithError(GeneralErrors.ValueIsInvalid("Departments must be unique"));
    }    
}