using Microsoft.AspNetCore.Http;

namespace Shared.Framework.EndpointResults;

public record SuccessResult<TValue> : IResult
{
    private readonly TValue? _value;

    public SuccessResult(TValue? value)
    {
        _value = value;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);       
        
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        
        var envelope = Envelope<TValue>.Ok(_value);
        return httpContext.Response.WriteAsJsonAsync(envelope);       
    }
}