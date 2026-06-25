using CSharpFunctionalExtensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions;

public interface ICommand;

/// <summary>
/// Represents a handler for processing a command of type <typeparamref name="TCommand"/> and returning a result of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the result produced by the command handler.</typeparam>
/// <typeparam name="TCommand">The type of the command being handled, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<TResponse, in TCommand> where TCommand : ICommand
{
    Task<Result<TResponse, Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a handler for processing a command of type <typeparamref name="TCommand"/>
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<UnitResult<Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}