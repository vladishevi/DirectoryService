using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTest()
    {
        return Ok("testOk");
    }
}