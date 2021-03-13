using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using smarthome.mqttService.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace smarthome.mqttService
{
    public interface IAmqpClient
    {
        public void SendMessageToQueue(string message);
    }
}
