using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace ProjetoBanco.API.Messaging
{
    public interface IRabbitMqService
    {
        void PublishContratacao(object message);
    }

    public class RabbitMqService : IRabbitMqService
    {
        private readonly string _hostname = "localhost";
        private readonly string _queueName = "contratacoes_queue";

        public void PublishContratacao(object message)
        {
            try 
            {
                var factory = new ConnectionFactory() { HostName = _hostname };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var json = JsonSerializer.Serialize(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                                         routingKey: _queueName,
                                         basicProperties: null,
                                         body: body);
                }
            }
            catch
            {
                // Em um ambiente real, trataríamos a falha de conexão com o RabbitMQ
                // Para fins de demonstração e compilação, apenas ignoramos ou logamos
            }
        }
    }
}
