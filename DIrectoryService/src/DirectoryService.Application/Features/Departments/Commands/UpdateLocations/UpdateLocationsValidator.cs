using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Commands;

public class UpdateLocationsValidator : AbstractValidator<UpdateLocationsCommand>
{
    public UpdateLocationsValidator()
    {
        RuleFor(l => l.Request.LocationIds)
            .Must(l => l.Distinct().Count() == l.Count()).WithError(GeneralErrors.ValueIsInvalid("Locations must be unique"));       
    }
}