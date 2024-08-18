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
         ClientProvidedName = "someUser",
         UserName = "someUser",
         Password = "Password",
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
}
