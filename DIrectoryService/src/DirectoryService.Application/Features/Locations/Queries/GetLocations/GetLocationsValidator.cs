using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Features.Locations.GetLocations;

public class GetLocationsValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsValidator()
    {
        RuleFor(l => l.Request.Search).MaximumLength(100)
            .When(l => !string.IsNullOrWhiteSpace(l.Request.Search))
            .WithError(GeneralErrors.ValueIsInvalid("Search", "Search must be less than 100 characters"));

        RuleFor(l => l.Request.MinDepartmentCount).GreaterThanOrEqualTo(0)
            .WithError(GeneralErrors.ValueIsInvalid("MinDepartmentCount",
                "MinDepartmentCount must be greater than or equal to 0"));
        
        RuleFor(l => l.Request.Pagination.Page).GreaterThanOrEqualTo(1)
            .WithError(GeneralErrors.ValueIsInvalid("Page", "Page must be greater than or equal to 1"));

        RuleFor(l => l.Request.Pagination.PageSize).InclusiveBetween(1, 100)
            .WithError(GeneralErrors.ValueIsInvalid("PageSize", "Page size must be between 1 and 100"));

        RuleFor(l => l.Request.SortBy)
            .Must(s => string.Equals(s, "name", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "createdAt", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "departmentsCount", StringComparison.InvariantCultureIgnoreCase))
            .WithError(GeneralErrors.ValueIsInvalid("SortBy", "SortBy must be either name, created_at or departmentsCount"));
        
        RuleFor(l => l.Request.SortDir)
            .Must(s => string.Equals(s, "asc", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "desc", StringComparison.InvariantCultureIgnoreCase))
            .WithError(GeneralErrors.ValueIsInvalid("SortDir", "SortDir must be either Asc or Desc"));       
   
    }
}