using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _validator;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> validator,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }
    
    /// <summary>
    /// Creates a new location based on the specified request and saves it to the repository.
    /// </summary>
    /// <param name="locationDto">The location creation request containing necessary details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A result containing either the unique identifier of the created location
    /// on success or an <see cref="Errors"/> object if the operation fails.
    /// </returns>
    public async Task<Result<Guid, Errors>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command.CreateLocationDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            Errors errors = new([.. validationResult.Errors.Select(e => GeneralErrors.ValueIsInvalid(e.PropertyName, e.ErrorMessage))]);
            return errors;
        }
        
        //Валидация сущности
        Result<Name, Errors> nameResult = Name.Create(command.CreateLocationDto.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }
        
        Result<Address, Errors> addressResult = Address.Create(
            command.CreateLocationDto.Address.City,
            command.CreateLocationDto.Address.Street,
            command.CreateLocationDto.Address.Building,
            command.CreateLocationDto.Address.Postcode);
        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }
        
        Result<Timezone, Errors> timezoneResult = Timezone.Create(command.CreateLocationDto.Timezone);
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

    public Task<Result<Guid, Errors>> Handle(CreateLocationCommand command) => throw new NotImplementedException();
}