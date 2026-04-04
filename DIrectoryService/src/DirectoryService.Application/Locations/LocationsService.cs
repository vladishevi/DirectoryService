using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations;

/// <summary>
/// 
/// </summary>
public class LocationsService
{
    private readonly ILocationsRepository _locationsRepository;

    public LocationsService(ILocationsRepository locationsRepository)
    {
        _locationsRepository = locationsRepository;
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
    }
}