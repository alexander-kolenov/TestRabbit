namespace MyRabbit.Rabbit;


public interface IRabbitMqService
{
   T? ReceiveMessage<T>(string queue);
   void SendMessage(string exchange, string routingKey, object obj);
}
