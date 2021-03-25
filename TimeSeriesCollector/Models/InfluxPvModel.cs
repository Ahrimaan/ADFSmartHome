namespace TimeSeriesCollector.Models
{
    using System;
    using InfluxDB.Client.Core;

    [Measurement("energy")]
    public class InfluxPvModel
    {
        [Column(IsTimestamp = true)]
        public DateTime Date { get { return _date.ToUniversalTime(); } set { _date = value; } }

        private DateTime _date;

        [Column("pvCurrent")]
        public decimal PvCurrent { get; set; }
        [Column("battery")]
        public int Battery { get; set; }
        [Column("consumption")]
        public decimal Consumption { get; set; }
        [Column("external")]
        public decimal External { get; set; }

        [Column("batteryLoad")]
        public decimal BatteryLoad { get; set; }

        [Column("tracker_voltage_1")]
        public int TrackerVoltage1 { get; set; }

        [Column("tracker_voltage_2")]
        public int TrackerVoltage2 { get; set; }

        [Column("tracker_voltage_3")]
        public int TrackerVoltage3 { get; set; }

        [Column("tracker_power_1")]
        public decimal TrackerPower1 { get; set; }

        [Column("tracker_power_2")]
        public decimal TrackerPower2 { get; set; }
    }
}