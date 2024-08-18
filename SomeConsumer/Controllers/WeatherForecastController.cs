using DomainModels;
using Microsoft.AspNetCore.Mvc;
using MyRabbit.Rabbit;

namespace SomeConsumer.Controllers
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

      [HttpGet(Name = "GetWeather")]
      public WeatherForecast? Get(string queue)
      {
         var messasge = _rabbitMqService.ReceiveMessage<WeatherForecast>(queue);
         return messasge;
      }
   }
}
