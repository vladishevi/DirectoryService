using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations;

/// <summary>
/// 
/// </summary>
public class LocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(ILocationsRepository locationsRepository, ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        //проверка валидности
        //создание сущности
        Name name = Name.Create(locationDto.Name).Value;
        Address address = Address
            .Create(locationDto.City, locationDto.Street, locationDto.Building, locationDto.Postcode).Value;
        Timezone timezone = Timezone.Create(locationDto.Timezone).Value;
        
        Location location = new Location(name, address, timezone);
        
        //сохранение в бд
        await _locationsRepository.AddAsync(location, cancellationToken);
        //логирование
        _logger.LogInformation("Location created with id: {id}", location.Id);
    }
}