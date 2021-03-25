using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TimeSeriesCollector.Contracts;

namespace TimeSeriesCollector
{
    public class AmqpWorker:BackgroundService
    {
        private readonly ILogger<AmqpWorker> _logger;
        private readonly IInfluxClient _influx;
        private readonly IAmqpClient _amqp;

        public AmqpWorker(ILogger<AmqpWorker> logger, IAmqpClient amqp, IInfluxClient influxClient)
        {
            _logger = logger;
            _influx = influxClient;
            _amqp = amqp;
            _amqp.SubscribeToEvents(RecieveMessageAsync);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }

        void RecieveMessageAsync(string data, BasicDeliverEventArgs args)
        {
            _logger.LogInformation($"Recieved from AMQP Client: {data}");
            var message = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            message.Add("key", args.RoutingKey);
            var result  = _influx.StoreData(message).Result;
            if (result)
            {
                _amqp.AcknowledgeMessage(args.DeliveryTag);
            } else
            {
                _amqp.RejectMessage(args.DeliveryTag);
            }
            _logger.LogInformation($"Stored data to Influx");
        }
    }
}
