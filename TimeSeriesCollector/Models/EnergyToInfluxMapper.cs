namespace TimeSeriesCollector.Models
{
    using TimeSeriesCollector.Contracts;
    public class EnergyToInfluxMapper : IModelMapper<EnergyModel, InfluxEnergyModel>
    {
        public InfluxEnergyModel MapTo(EnergyModel model)
        {
            var dbModel = new InfluxEnergyModel
            {
                Current = model.ENERGY.Current,
                Device = model.Device,
                Power = model.ENERGY.Power,
                Time = model.Time,
                Today = model.ENERGY.Today,
                Total = model.ENERGY.Total,
                TotalStartTime =model.ENERGY.TotalStartTime,
                Voltage = model.ENERGY.Voltage,
                Yesterday = model.ENERGY.Yesterday
            };
            return dbModel;
        }
    }
}
