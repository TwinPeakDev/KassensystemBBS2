using Microsoft.AspNetCore.Mvc;

namespace Kassensystem.Controllers;

[ApiController]
[Route("[controller]")]
public class PathController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PathController> _logger;

    public PathController(IWebHostEnvironment environment, ILogger<PathController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet("wwwroot")]
    public IActionResult GetWebRoot()
    {
        return StatusCode(200, _environment.WebRootPath);
    }
}