using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Shared.Errors;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Presenters.EndpointResults;

public record EndpointResult<TValue> : IResult
{
    private readonly IResult _result;

    private EndpointResult(Result<TValue, Errors> result)
    {
        _result = result.IsFailure
            ? new ErrorResult(result.Error)
            : new SuccessResult<TValue>(result.Value);
    }

    public Task ExecuteAsync(HttpContext httpContext) => 
        _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Errors> result) => new(result);
}