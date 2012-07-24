using System;
using System.Runtime.Serialization;

namespace BrewBuddy.Worker.Logic
{
    [DataContract(Name = "SensorReadingV1", Namespace = "urn:api.brewbuddy.net")]
    public class SensorReading
    {
        [DataMember]
        public string SensorId { get; set; }

        [DataMember]
        public DateTime When { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}