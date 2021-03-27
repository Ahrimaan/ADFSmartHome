using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using smarthome.mqttService.Config;
using smarthome.mqttService.Contracts;
using System.Text;
using System.Text.Json;

namespace smarthome.mqttService
{
    public class AmqpClient : IAmqpClient
    {
        IModel _channel;
        AmqpOptions _options;
        ILogger<MqttWorker> _logger;

        public AmqpClient(AmqpOptions options, ILogger<MqttWorker> logger)
        {
            _logger = logger;
            _options = options;
            _channel = GetRabbitChannel(options);
        }

        public void SendMessageToQueue(string message)
        {
            _logger.LogInformation($"Sending Data to the Queue {message}");
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: _options.Exchange,
                                 routingKey: _options.RoutingKey,
                                 basicProperties: null,
                                 body: body);
        }

        private IModel GetRabbitChannel(AmqpOptions options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.Server,
                Password = options.Password,
                UserName = options.User,
                Port = options.Port
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ModelShutdown += Channel_ModelShutdown;
            return channel;
        }

        private void Channel_ModelShutdown(object sender, ShutdownEventArgs e)
        {
            // If channel closes we open it again
            _channel = GetRabbitChannel(_options);
        }
    }
}
