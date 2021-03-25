namespace smarthome.mqttService.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EnergyMessage
    {
        public DateTime Time { get; set; }
        public Energy ENERGY { get; set; }

        public string Device { get; set; }

        private class FlattenedModel
        {
            public string Device { get; set; }
            public DateTime Time { get; set; }

            public DateTime TotalStartTime { get; set; }
            public decimal Total { get; set; }
            public decimal Yesterday { get; set; }
            public decimal Today { get; set; }
            public decimal Power { get; set; }
            public int Voltage { get; set; }
            public decimal Current { get; set; }
        }

        public IDictionary<string,object> ToDic()
        {
            var flat = new FlattenedModel
            {
                Current = this.ENERGY.Current,
                Device = this.Device,
                Power = this.ENERGY.Power,
                Time = this.Time,
                Today = this.ENERGY.Today,
                Total = this.ENERGY.Total,
                TotalStartTime = this.ENERGY.TotalStartTime,
                Voltage = this.ENERGY.Voltage,
                Yesterday = this.ENERGY.Yesterday
            };
            return flat.AsDictionary();
        }
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

    public static class ObjectExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
