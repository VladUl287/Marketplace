using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Product.Api.Options;

namespace Product.Api.Controllers;

[ApiController, Route("[controller]/[action]")]
public sealed class ConfigurationController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromServices] IOptions<DbOptions> options)
    {
        return Ok(options.Value);
    }

    [HttpGet]
    public IActionResult GetMonitor([FromServices] IOptionsMonitor<DbOptions> options)
    {
        return Ok(options.CurrentValue);
    }
}
