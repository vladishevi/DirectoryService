using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters.EndpointResults;

public record EndpointResult : IActionResult
{
    private readonly IActionResult _result;

    private EndpointResult(object result)
    {
        _result = new SuccessResult(result);      
    }

    private EndpointResult(Errors errors)
    {
        _result = new ErrorResult(errors);       
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        await _result.ExecuteResultAsync(context);       
    }

    public static EndpointResult Success(object result) => new(result);
    public static EndpointResult Error(Errors errors) => new(errors);
}