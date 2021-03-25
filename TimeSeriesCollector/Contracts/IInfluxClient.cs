using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSeriesCollector.Models;

namespace TimeSeriesCollector.Contracts
{
    public interface IInfluxClient
    {
        public Task<bool> StoreData(Dictionary<string,string> values);
    }
}
