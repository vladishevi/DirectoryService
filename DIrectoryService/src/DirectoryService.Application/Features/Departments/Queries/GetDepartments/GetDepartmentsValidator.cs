using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsValidator : AbstractValidator<GetDepartmentsQuery>
{
    public GetDepartmentsValidator()
    {
        RuleFor(d => d.Request.Search).MaximumLength(100)
            .WithError(GeneralErrors.ValueIsInvalid("Search", "Search must be less than 100 characters"));

        RuleFor(d => d.Request.Pagination.Page).GreaterThanOrEqualTo(1)
            .WithError(GeneralErrors.ValueIsInvalid("Page", "Page must be greater than or equal to 1"));

        RuleFor(d => d.Request.Pagination.PageSize).InclusiveBetween(1, 100)
            .WithError(GeneralErrors.ValueIsInvalid("PageSize", "Page size must be between 1 and 100"));

        RuleFor(d => d.Request.SortBy)
            .Must(s => string.Equals(s, "Name", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "CreatedAt", StringComparison.InvariantCultureIgnoreCase))
            .WithError(GeneralErrors.ValueIsInvalid("SortBy", "SortBy must be either Name or CreatedAt"));
        
        RuleFor(d => d.Request.SortDir)
            .Must(s => string.Equals(s, "Asc", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "Desc", StringComparison.InvariantCultureIgnoreCase))
            .WithError(GeneralErrors.ValueIsInvalid("SortDir", "SortDir must be either Asc or Desc"));       
    }
}