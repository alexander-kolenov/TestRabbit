namespace SomeWorker;

using Microsoft.Extensions.DependencyInjection;
using MyRabbit;

public class Program
{
   public static async Task Main(string[] args)
   {
      var builder = Host.CreateApplicationBuilder(args);

      builder.Services.AddMyRabbit();
      builder.Services.AddHostedService<Worker>();



      var host = builder.Build();
      try { 
         await host.StartAsync();
         await host.WaitForShutdownAsync();
         await host.StopAsync();
      }
      finally
      {
         Console.WriteLine("Main: stopping");

         if (host is IAsyncDisposable d) await d.DisposeAsync();
      }
   }
}