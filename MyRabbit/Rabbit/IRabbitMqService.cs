namespace MyRabbit.Rabbit;


public interface IRabbitMqService
{
   T? ReceiveMessage<T>(string queue);
   void SendMessage(string exchange, string routingKey, object obj);
   public IDisposable Consume(string queueName, Action<string> action);
}
