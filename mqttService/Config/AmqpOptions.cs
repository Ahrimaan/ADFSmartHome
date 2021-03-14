namespace smarthome.mqttService.Config
{
    public class AmqpOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }

        public string Password { get; set; }

        public string Exchange { get; set; }
    }
}
