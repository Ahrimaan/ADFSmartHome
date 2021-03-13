using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using smarthome.mqttService.Config;
using System.Text;
using System.Text.Json;

namespace smarthome.mqttService
{
    public class AmqpClient : IAmqpClient
    {
        IModel channel;
        string routingKey;
        string exchange;

        public AmqpClient(AmqpOptions options, ILogger<MqttWorker> logger)
        {
            var serialized = JsonSerializer.Serialize(options);
            logger.LogInformation(serialized);
            this.routingKey = options.Queue;
            this.exchange = options.Exchange;
            var factory = new ConnectionFactory()
            {
                HostName = options.Server,
                Password = options.Password,
                UserName = options.User,
                Port = options.Port
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(options.Exchange, ExchangeType.Fanout, durable: true);
            this.channel = channel;
        }

        public void SendMessageToQueue(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
