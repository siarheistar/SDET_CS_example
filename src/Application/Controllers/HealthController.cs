using Microsoft.AspNetCore.Mvc;

namespace SDET.Application.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "SDET API",
            version = "1.0.0"
        });
    }
}
