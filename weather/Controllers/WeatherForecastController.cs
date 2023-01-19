using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace weather.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    private String GetSubject()
    {
        return GetClaim(ClaimTypes.NameIdentifier);
    }
    private String GetClaim(String type)
    {
        Claim c = User.Claims.FirstOrDefault(c => c.Type == type);
        return c?.Value;
    }

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult Authenticated()
    {
        return Ok(new { data = "Some data from secured endpoint.", user = GetSubject() });
    }

    [HttpGet("developer")]
    [Authorize(Policy = "developer")]
    public IActionResult Developer()
    {
        return Ok(new { data = "Some data for developers", user = GetSubject(), title = GetClaim("title") });
    }

    [HttpGet("lowrisk")]
    [Authorize(Policy = "lowRisk")]
    public IActionResult LowRisk()
    {
        return Ok(new { data = "Your risk score is low", user = GetSubject(), risk = GetClaim("risk") });
    }
}
