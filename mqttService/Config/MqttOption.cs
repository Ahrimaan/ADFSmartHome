namespace smarthome.mqttService.Config
{
    public class MqttOption
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public string[] Topics { get; set; }

        public bool Resubscribe { get; set; }

        public int KeepAlive { get; set; }

        public int ConnectionTimeout { get; set; }
    }
}
