using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Features.Locations.GetLocations;

public class GetLocationsValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsValidator()
    {
        
        /*
        RuleFor(l => l.Request.Search).MaximumLength(100)
            .WithError(GeneralErrors.ValueIsInvalid("Search", "Search must be less than 100 characters"));
            */

        RuleFor(l => l.Request.Pagination.Page).GreaterThanOrEqualTo(1)
            .WithError(GeneralErrors.ValueIsInvalid("Page", "Page must be greater than or equal to 1"));

        RuleFor(l => l.Request.Pagination.PageSize).InclusiveBetween(1, 100)
            .WithError(GeneralErrors.ValueIsInvalid("PageSize", "Page size must be between 1 and 100"));

        /*
        RuleFor(l => l.Request.SortBy)
            .Must(s => string.Equals(s, "Name", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "CreatedAt", StringComparison.InvariantCultureIgnoreCase))
            .WithError(GeneralErrors.ValueIsInvalid("SortBy", "SortBy must be either Name or CreatedAt"));
        
        RuleFor(l => l.Request.SortDir)
            .Must(s => string.Equals(s, "Asc", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(s, "Desc", StringComparison.InvariantCultureIgnoreCase))
            .WithError(GeneralErrors.ValueIsInvalid("SortDir", "SortDir must be either Asc or Desc"));       */
   
    }
}