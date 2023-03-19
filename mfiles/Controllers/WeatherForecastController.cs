using mfiles.BO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;

namespace mfiles.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    
   
    [HttpGet("Get Person by id")]
    public async Task<Person> GetPerson(int id)
    {
        if (id == 12)
        {
            Person p = new Person()
            {
                Id = 12,
                FirstName = "Annie",
                SecondName = "Cruz",
                BirthDate = new(1984, 11, 6)
            };
            return p;
        }

        return null;
    }

    
    [HttpGet("Get person photo by id")]
    public async Task<IActionResult> GetPhoto(int id)
    {
        if (id == 12)
            return File(await System.IO.File.ReadAllBytesAsync(Path.Combine("files", "girl.jpg")), "image/jpeg", "girl.jpg");
        return BadRequest();
    }

    
}
