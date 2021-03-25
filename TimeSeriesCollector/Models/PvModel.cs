using System;
namespace TimeSeriesCollector.Models
{
    public class PvModel
    {
        public DateTime Date { get; set; }

        public decimal PvCurrent { get; set; }

        public int Battery { get; set; }

        public decimal Consumption { get; set; }

        public decimal External { get; set; }

        public decimal BatteryLoad { get; set; }

        public int TrackerVoltage1 { get; set; }

        public int TrackerVoltage2 { get; set; }

        public int TrackerVoltage3 { get; set; }

        public decimal TrackerPower1 { get; set; }

        public decimal TrackerPower2 { get; set; }
    }
}
