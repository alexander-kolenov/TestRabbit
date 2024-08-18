using DomainModels;
using Microsoft.AspNetCore.Mvc;
using MyRabbit.Rabbit;

namespace SomeProducer.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class WeatherForecastController : ControllerBase
   {
      private readonly ILogger<WeatherForecastController> _logger;
      private readonly IRabbitMqService _rabbitMqService;

      public WeatherForecastController(
         ILogger<WeatherForecastController> logger,
         IRabbitMqService rabbitMqService)
      {
         _logger = logger;
         _rabbitMqService = rabbitMqService;
      }

      [HttpPost(Name = "PostWeatherForecast")]
      public WeatherForecast PostWeatherForecast(string routingKey, int temperature)
      {
         var weather = new WeatherForecast
         {
            Date = DateTime.UtcNow,
            TemperatureC = temperature,
            Summary = "someProducer"
         };

         _rabbitMqService.SendMessage("amq.direct", routingKey, weather);
         return weather;
      }
   }
}
