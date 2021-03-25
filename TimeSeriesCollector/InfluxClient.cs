using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSeriesCollector.Config;
using TimeSeriesCollector.Contracts;
using TimeSeriesCollector.Models;

namespace TimeSeriesCollector
{
    public class InfluxClient : IInfluxClient
    {
        private readonly ILogger<AmqpWorker> _logger;
        private readonly InfluxDbOptions _options;

        public InfluxClient(InfluxDbOptions options, ILogger<AmqpWorker> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<bool> StoreData(Dictionary<string, string> values)
        {
            try
            {
                var dbWriter = InfluxDBClientFactory.Create(_options.InfluxUri, _options.Token.ToCharArray());
                dbWriter.SetLogLevel(InfluxDB.Client.Core.LogLevel.Body);
                var writeApi = dbWriter.GetWriteApiAsync();
                var measurment = values["key"];
                var point = PointData.Measurement(measurment);
                foreach (var item in values.Where(x => x.Key != "key"))
                {
                    point.Field(item.Key, item.Value);
                }
                await writeApi.WritePointAsync(point);
                _logger.LogInformation($"Success writing influxdb for {measurment}");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not write to influx");
                return false;
            }
        }
    }
}
