using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters.EndpointResults;

public record ErrorResult : IActionResult
{
    private readonly Errors _errors;

    public ErrorResult(Errors errors)
    {
        _errors = errors;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = GetStatusCode();        

        Envelope envelope = Envelope.Error(_errors);
        await context.HttpContext.Response.WriteAsJsonAsync(envelope);
    }

    private int GetStatusCode()
    {
        return !_errors.Any() 
            ? StatusCodes.Status500InternalServerError 
            : GetStatusCodeByErrorType(_errors.First());
    }

    private int GetStatusCodeByErrorType(Error error)
    {
        switch (error.Type)
        {
            case ErrorType.VALIDATION:
                return StatusCodes.Status400BadRequest;
            case ErrorType.NOT_FOUND:
                return StatusCodes.Status404NotFound;
            case ErrorType.CONFLICT:
                return StatusCodes.Status409Conflict;
            default:
                return StatusCodes.Status500InternalServerError;
        }
    }
}