using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace MyRabbit.Rabbit;


public class RabbitMqService : IRabbitMqService
{
   private ConnectionFactory CreateConnectionFactory()
   {
      var factory = new ConnectionFactory()
      {
         HostName = "rabbitmq",
         ClientProvidedName = Guid.NewGuid().ToString(),
         UserName = "rmuser",
         Password = "rmpassword",
         Port = 5672,
      };
      return factory;
   }


   public T? ReceiveMessage<T>(string queue)
   {
      var factory = CreateConnectionFactory();
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {

         var consumer = new EventingBasicConsumer(channel);
         var result = channel.BasicGet(queue: queue, autoAck: true);
         if (result is null) return default;

         var message = JsonSerializer.Deserialize<T>(result.Body.ToArray());
         return message;
      }
   }
   
   public void SendMessage(string exchange, string routingKey, object obj)
   {
      var factory = CreateConnectionFactory();
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
         var message = JsonSerializer.Serialize(obj);
         var body = Encoding.UTF8.GetBytes(message);

         channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body);

      }
   }

   public IDisposable Consume(string queueName, Action<string> action)
   {
      var factory = CreateConnectionFactory();
      var connection = factory.CreateConnection();
      var channel = connection.CreateModel();
      
      channel.QueueDeclare(queue: queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

      var consumer = new EventingBasicConsumer(channel);



      consumer.Received += Consumer_Received;
      channel.BasicConsume(queue: queueName,
                           autoAck: true,
                           consumer: consumer);
      return new Disp(() =>
      {
         consumer.Received -= Consumer_Received;
         channel.Dispose();
         connection.Dispose();
      });

      void Consumer_Received (object? sender, BasicDeliverEventArgs e)
      {
         byte[] body = e.Body.ToArray();
         var message = Encoding.UTF8.GetString(body);
         action(message);
      };
   }



   private record Disp(Action action)
      : IDisposable
   {
      private bool isDisposed;
      public void Dispose()
      {
         if(!isDisposed)
            action();
      }
   }
}
