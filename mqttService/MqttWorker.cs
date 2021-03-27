using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using smarthome.mqttService.Config;
using smarthome.mqttService.Contracts;
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
        private MqttClient _client;
        private TopicDictionary _dic;
        private IAmqpClient _amqp;
        private string _clientId => "mqttService";

        public MqttWorker(MqttOption options, ILogger<MqttWorker> logger, TopicDictionary dict, IAmqpClient amqpClient)
        {
            _amqp = amqpClient;
            _dic = dict;
            _logger = logger;
            _client = GetMqttClient(options.Server,options.Topics);
        }

        private MqttClient GetMqttClient(string server, string[] topics)
        {
            var client = new MqttClient(server);
            client.Connect(_clientId);
            client.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            client.Subscribe(topics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            return client;
        }

        private void Client_ConnectionClosed(object sender, System.EventArgs e)
        {
            _client.Connect(_clientId);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                _client.Disconnect();
            }
            await Task.Delay(100);
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = JsonSerializer.Deserialize<EnergyMessage>(e.Message);
            message.Device = _dic.Mapping.GetValueOrDefault(e.Topic);
            var dic = message.ToDic();
            var serialized = JsonSerializer.Serialize(dic);
            _logger.LogInformation(serialized);
            _amqp.SendMessageToQueue(serialized);
        }
    }
}
