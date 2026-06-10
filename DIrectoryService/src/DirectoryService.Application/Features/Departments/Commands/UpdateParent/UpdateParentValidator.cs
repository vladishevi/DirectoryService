using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Commands;

public class UpdateParentValidator : AbstractValidator<UpdateParentCommand>
{
    public UpdateParentValidator()
    {
        RuleFor(d => d)
            .Must(d => d.DepartmentId != d.Request.ParentId)
            .WithMessage("Department cannot be its own parent");       
    }
}