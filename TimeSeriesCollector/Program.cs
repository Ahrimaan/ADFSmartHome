using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeSeriesCollector.Config;
using TimeSeriesCollector.Contracts;
using TimeSeriesCollector.Models;

namespace TimeSeriesCollector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AmqpOptions amqtOptions = configuration.GetSection("amqp").Get<AmqpOptions>();
                    InfluxDbOptions influxOptions = configuration.GetSection("influx").Get<InfluxDbOptions>();
                    services.AddSingleton(typeof(IAmqpClient), typeof(AmqpClient));
                    services.AddSingleton(typeof(IInfluxClient), typeof(InfluxClient));
                    services.AddSingleton(amqtOptions);
                    services.AddSingleton(influxOptions);
                    services.AddHostedService<AmqpWorker>();
                });
    }
}
