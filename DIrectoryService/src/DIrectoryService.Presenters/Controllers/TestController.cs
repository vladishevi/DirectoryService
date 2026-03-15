using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public void Test()
    {
    }
}