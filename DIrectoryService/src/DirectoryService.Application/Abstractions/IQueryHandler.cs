using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstractions;

public interface IQuery;

/// <summary>
/// Represents a handler for processing a query of type <typeparamref name="TQuery"/> and returning a result of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the result produced by the query handler.</typeparam>
/// <typeparam name="TQuery">The type of the query being handled, which must implement <see cref="IQuery"/>.</typeparam>
public interface IQueryHandler<TResponse, in TQuery> where TQuery : IQuery
{
    Task<Result<TResponse, Errors>> Handle(TQuery query, CancellationToken cancellationToken);
}

public interface IQueryHandler<TResponse>
{
    Task<Result<TResponse, Errors>> Handle(CancellationToken cancellationToken);
}
