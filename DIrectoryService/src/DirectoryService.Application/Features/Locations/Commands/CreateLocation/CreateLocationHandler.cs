using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Errors;

namespace DirectoryService.Application.Features.Locations.Commands;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationCommand> _validator;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(ILocationsRepository locationsRepository,
        IValidator<CreateLocationCommand> validator,
        ITransactionManager transactionManager,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
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
        ValidationResult validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        Result<Name, Errors> nameResult = Name.Create(command.CreateLocationRequest.Name);
        Result<Address, Errors> addressResult = Address.Create(
            command.CreateLocationRequest.Address.City,
            command.CreateLocationRequest.Address.Street,
            command.CreateLocationRequest.Address.Building,
            command.CreateLocationRequest.Address.Postcode);
        
        Result<Timezone, Errors> timezoneResult = Timezone.Create(command.CreateLocationRequest.Timezone);
        Location location = new(nameResult.Value, addressResult.Value, timezoneResult.Value);

        //add location
        Result<Guid, Errors> result = await _locationsRepository.Add(location, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("Error creating location: {error}", result.Error);
            return result.Error;
        }
        
        //db save
        UnitResult<Errors> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Error saving location to database: {error}", saveResult.Error);
            return saveResult.Error;
        }
                
        //logging
        _logger.LogInformation("Location created with id: {id}", location.Id);
        return location.Id;
    }
}