using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.EndpointResults;

public record SuccessResult : IActionResult
{
    private readonly object _result;

    public SuccessResult(object result)
    {
        _result = result;       
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        
        Envelope envelope = Envelope.Ok(_result);
        
        await context.HttpContext.Response.WriteAsJsonAsync(envelope);       
    }
}