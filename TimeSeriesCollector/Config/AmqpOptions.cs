namespace TimeSeriesCollector.Config
{
    public class AmqpOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }

        public string Password { get; set; }

        public string Queue { get; set; }

        public bool Durable { get; set; }

        public bool Requeue { get; set; }

        public string Exchange { get; set; }
    }
}
