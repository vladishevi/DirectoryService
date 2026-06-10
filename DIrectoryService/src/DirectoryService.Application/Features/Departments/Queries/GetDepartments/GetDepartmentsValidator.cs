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
            .Must(s => Enum.IsDefined(typeof(SortBy), s))
            .WithError(GeneralErrors.ValueIsInvalid("SortBy", "SortBy must be a valid value"));
    }
}