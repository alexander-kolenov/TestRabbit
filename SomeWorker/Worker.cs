using MyRabbit.Rabbit;

namespace SomeWorker
{
   public class Worker : BackgroundService
   {
      private readonly ILogger<Worker> _logger;
      private readonly IServiceScopeFactory _serviceScopeFactory;

      public Worker(
         ILogger<Worker> logger,
         IServiceScopeFactory serviceScopeFactory)
      {
         _logger = logger;
         _serviceScopeFactory = serviceScopeFactory;
      }

      protected override async Task ExecuteAsync(CancellationToken ct)
      {
         await Task.Delay(4000, ct);
         _logger.LogInformation("Worker start at: {time}", DateTimeOffset.UtcNow);

         using var scope = _serviceScopeFactory.CreateScope();
         var rabbitMqService = scope.ServiceProvider.GetService<IRabbitMqService>()!;
         using var consumer = rabbitMqService.Consume(
               "log",
               message => _logger.LogInformation($"{DateTimeOffset.UtcNow}: {message}"));

         while (!ct.IsCancellationRequested)
         {
            try { await Task.Delay(1000, ct); } catch (Exception) { }
         }
         _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.UtcNow);
      }

   }
}
