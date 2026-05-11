using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class UpdateParentHandler : ICommandHandler<UpdateParentCommand>
{
    public Task<Result<Guid, Errors>> Handle(UpdateParentCommand command, CancellationToken cancellationToken)
    {
        //validate command
        ///...
    }
}