using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class UpdateLocationsHandler : ICommandHandler<UpdateLocationsCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<UpdateLocationsHandler> _logger;

    public UpdateLocationsHandler(IDepartmentsRepository departmentsRepository, ILogger<UpdateLocationsHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
    }
    
    public Task<UnitResult<Errors>> Handle(UpdateLocationsCommand command, CancellationToken cancellationToken)
    {
        
    }
}