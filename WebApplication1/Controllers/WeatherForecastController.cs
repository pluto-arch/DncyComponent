using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITA _iTA;

        private readonly ISender _mediator;
        private readonly DemoContext demoContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ISender mediator, ITA iTa, DemoContext demoContext)
        {
            _logger = logger;
            _mediator = mediator;
            _iTA = iTa;
            this.demoContext = demoContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogInformation($"111 . {demoContext.GetHashCode()}");
            var products = await _mediator.Send(new GetAllProductsQuery());

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}