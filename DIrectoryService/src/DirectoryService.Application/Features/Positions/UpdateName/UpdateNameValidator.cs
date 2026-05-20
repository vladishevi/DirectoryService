using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;

namespace DirectoryService.Presenters.Features.Positions;

public class UpdateNameValidator : AbstractValidator<UpdateNameCommand>
{
    public UpdateNameValidator()
    {
        RuleFor(p => p.Request.Name)
            .MustBeValueObject(Name.Create);
    }   
}