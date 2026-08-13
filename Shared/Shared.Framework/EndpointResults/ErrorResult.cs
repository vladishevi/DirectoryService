using Microsoft.AspNetCore.Http;
using Shared.Errors;

namespace Shared.Framework.EndpointResults;

public record ErrorResult : IResult
{
    private readonly Errors.Errors _errors;

    public ErrorResult(Errors.Errors errors)
    {
        _errors = errors;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);      
        
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = GetStatusCode();        

        Envelope<object> envelope = Envelope<object>.Error(_errors);
        return httpContext.Response.WriteAsJsonAsync(envelope);
    }

    private int GetStatusCode()
    {
        return !_errors.Any() 
            ? StatusCodes.Status500InternalServerError 
            : GetStatusCodeByErrorType(_errors.First());
    }

    private int GetStatusCodeByErrorType(Error error)
    {
        return error.Type switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}