namespace smarthome.mqttService.Models
{
    using System;
    public class EnergyMessage
    {
        public DateTime Time { get; set; }
        public Energy ENERGY { get; set; }

        public string Device { get; set; }
    }

    public class Energy
    {
        public DateTime TotalStartTime { get; set; }
        public decimal Total { get; set; }
        public decimal Yesterday { get; set; }
        public decimal Today { get; set; }
        public decimal Power { get; set; }
        public int Voltage { get; set; }
        public decimal Current { get; set; }
    }
}
