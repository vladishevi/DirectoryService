using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations;

public class LocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _createValidator;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> createValidator,
        ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<Result<Guid, string>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        //Валидация входных данных
        ValidationResult? validationResult = await _createValidator.ValidateAsync(locationDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
        }
        
        //Валидация сущности
        Result<Name, string> nameResult = Name.Create(locationDto.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }
        
        Result<Address, string> addressResult = Address.Create(locationDto.City, locationDto.Street, locationDto.Building, locationDto.Postcode);
        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }
        
        Result<Timezone, string> timezoneResult = Timezone.Create(locationDto.Timezone);
        if (timezoneResult.IsFailure)
        {
            return timezoneResult.Error;
        }
        
        Location location = new(nameResult.Value, addressResult.Value, timezoneResult.Value);

        //сохранение в бд
        Result<Guid, string> result = await _locationsRepository.Add(location, cancellationToken);
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