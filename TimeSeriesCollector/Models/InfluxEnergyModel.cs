namespace TimeSeriesCollector.Models
{
    using System;
    using InfluxDB.Client.Core;

    [Measurement("energy")]
    public class InfluxEnergyModel
    {
        [Column(IsTimestamp = true)]
        public DateTime Time { get { return _time.ToUniversalTime(); } set { _time = value; } }

        private DateTime _time;

        [Column("device", IsTag = true)]
        public string Device { get; set; }

        [Column("totalstarttime")]
        public DateTime TotalStartTime { get; set; }
        [Column("total")]
        public decimal Total { get; set; }
        [Column("yesterday")]
        public decimal Yesterday { get; set; }
        [Column("today")]
        public decimal Today { get; set; }

        [Column("power")]
        public decimal Power { get; set; }

        [Column("voltage")]
        public int Voltage { get; set; }

        [Column("current")]
        public decimal Current { get; set; }
    }
}
