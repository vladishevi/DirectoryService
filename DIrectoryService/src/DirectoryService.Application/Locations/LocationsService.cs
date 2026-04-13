using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations;

public class LocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationRequest> _createValidator;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(ILocationsRepository locationsRepository,
        IValidator<CreateLocationRequest> createValidator,
        ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Create(CreateLocationRequest locationRequest, CancellationToken cancellationToken)
    {
        //Валидация входных данных
        ValidationResult? validationResult = await _createValidator.ValidateAsync(locationRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            Errors errors = validationResult.Errors.Select(e => Error.Validation(e.ErrorMessage)).ToList();
            return errors;
        }
        
        //Валидация сущности
        Result<Name, Errors> nameResult = Name.Create(locationRequest.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }
        
        Result<Address, Errors> addressResult = Address.Create(
            locationRequest.Address.City,
            locationRequest.Address.Street,
            locationRequest.Address.Building,
            locationRequest.Address.Postcode);
        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }
        
        Result<Timezone, Errors> timezoneResult = Timezone.Create(locationRequest.Timezone);
        if (timezoneResult.IsFailure)
        {
            return timezoneResult.Error;
        }
        
        Location location = new(nameResult.Value, addressResult.Value, timezoneResult.Value);

        //сохранение в бд
        Result<Guid, Errors> result = await _locationsRepository.Add(location, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("Error creating location: {error}", result.Error);
            return result.Error;
        }
                
        //логирование
        _logger.LogInformation("Location created with id: {id}", location.Id);
        return location.Id;
    }

}
