using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Features.Departments.Commands;

public class UpdatePositionsValidator : AbstractValidator<UpdatePositionsCommand>
{
    public UpdatePositionsValidator()
    {
        RuleFor(p => p.Request.PositionIds)
            .Must(p => p.Distinct().Count() == p.Count()).WithError(GeneralErrors.ValueIsInvalid("Positions must be unique"));       
    }
}
