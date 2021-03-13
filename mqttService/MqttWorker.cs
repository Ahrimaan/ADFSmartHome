using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using smarthome.mqttService.Config;
using smarthome.mqttService.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace smarthome.mqttService
{
    public class MqttWorker : BackgroundService
    {
        private readonly ILogger<MqttWorker> _logger;
        private MqttClient client;
        private TopicDictionary _dic;
        private IAmqpClient _amqp;

        public MqttWorker(MqttOption options, ILogger<MqttWorker> logger, TopicDictionary dict, IAmqpClient amqpClient)
        {
            _amqp = amqpClient;
            _dic = dict;
            _logger = logger;
            client = new MqttClient(options.Server);
            client.Connect("mqttService");
            client.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            client.Subscribe(options.Topics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                client.Disconnect();
            }
            await Task.Delay(100);
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = JsonSerializer.Deserialize<EnergyMessage>(e.Message);
            message.Device = _dic.Mapping.GetValueOrDefault(e.Topic);
            var serialized = JsonSerializer.Serialize(message);
            _logger.LogInformation(serialized);
            _amqp.SendMessageToQueue(serialized);
        }
    }
}
