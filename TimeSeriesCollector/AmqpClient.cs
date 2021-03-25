using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using TimeSeriesCollector.Config;
using TimeSeriesCollector.Contracts;

namespace TimeSeriesCollector
{
    public class AmqpClient : IAmqpClient
    {
        private IModel _channel;
        private readonly ILogger<AmqpWorker> _logger;
        private readonly AmqpOptions _options;

        public AmqpClient(AmqpOptions options, ILogger<AmqpWorker> logger)
        {
            _logger = logger;
            _options = options;
            _channel = GetRabbitChannel(options);
            logger.LogInformation($"Connected to AMQP : {_channel.IsOpen}");
        }

        public void SubscribeToEvents(Action<string, BasicDeliverEventArgs> action)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                action(message, ea);
                _logger.LogDebug(" [x] Received {0}", message);
            };
            _channel.BasicConsume(queue: _options.Queue,
                                     autoAck: false,
                                     consumer: consumer);
        }

        public void AcknowledgeMessage(ulong deliveryTag)
        {
            _channel.BasicAck(deliveryTag, false);
        }

        public void RejectMessage(ulong deliveryTag)
        {
            _channel.BasicNack(deliveryTag, false, _options.Requeue);
        }

        private IModel GetRabbitChannel(AmqpOptions options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.Server,
                UserName = options.User,
                Password = options.Password,
                Port = options.Port
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: options.Queue,
                                     durable: options.Durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            channel.ModelShutdown += Channel_ModelShutdown;
            return channel;
        }

        private void Channel_ModelShutdown(object sender, ShutdownEventArgs e)
        {
            _channel = GetRabbitChannel(_options);
        }


    }
}
