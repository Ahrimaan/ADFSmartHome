using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using smarthome.mqttService.Config;

namespace smarthome.mqttService
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
                    MqttOption mqttOptions = configuration.GetSection("mqtt").Get<MqttOption>();
                    AmqpOptions amqtOptions = configuration.GetSection("amqp").Get<AmqpOptions>();
                    TopicDictionary topicDictionary = configuration.GetSection("dict").Get<TopicDictionary>();
                    
                    services.AddSingleton(topicDictionary);
                    services.AddSingleton(mqttOptions);
                    services.AddSingleton(amqtOptions);
                    services.AddSingleton(typeof(IAmqpClient), typeof(AmqpClient));
                    services.AddHostedService<MqttWorker>();
                });
    }
}
