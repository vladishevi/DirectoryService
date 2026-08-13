using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Shared.Framework.EndpointResults;

public record EndpointResult<TValue> : IResult
{
    private readonly IResult _result;

    private EndpointResult(Result<TValue, Errors.Errors> result)
    {
        _result = result.IsFailure
            ? new ErrorResult(result.Error)
            : new SuccessResult<TValue>(result.Value);
    }

    public Task ExecuteAsync(HttpContext httpContext) => 
        _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Errors.Errors> result) => new(result);
}