using Microsoft.Extensions.DependencyInjection;
using MyRabbit.Rabbit;

namespace MyRabbit;

public static class Bootstarpper
{
   public static IServiceCollection AddMyRabbit (this IServiceCollection services)
   {
      services.AddScoped<IRabbitMqService, RabbitMqService>();
      return services;
   }
}
