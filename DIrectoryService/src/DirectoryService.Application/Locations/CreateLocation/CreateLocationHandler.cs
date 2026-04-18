using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
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
    private readonly IValidator<CreateLocationRequest> _validator;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(ILocationsRepository locationsRepository,
        IValidator<CreateLocationRequest> validator,
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
        //Input validation
        ValidationResult validationResult = await _validator.ValidateAsync(command.CreateLocationRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        //Domain validation
        Result<Name, Errors> nameResult = Name.Create(command.CreateLocationRequest.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }
        
        Result<Address, Errors> addressResult = Address.Create(
            command.CreateLocationRequest.Address.City,
            command.CreateLocationRequest.Address.Street,
            command.CreateLocationRequest.Address.Building,
            command.CreateLocationRequest.Address.Postcode);
        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }
        
        Result<Timezone, Errors> timezoneResult = Timezone.Create(command.CreateLocationRequest.Timezone);
        if (timezoneResult.IsFailure)
        {
            return timezoneResult.Error;
        }
        
        Location location = new(nameResult.Value, addressResult.Value, timezoneResult.Value);

        //db saving
        Result<Guid, Errors> result = await _locationsRepository.Add(location, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("Error creating location: {error}", result.Error);
            return result.Error;
        }
                
        //logging
        _logger.LogInformation("Location created with id: {id}", location.Id);
        return location.Id;
    }

    public Task<Result<Guid, Errors>> Handle(CreateLocationCommand command) => throw new NotImplementedException();
}