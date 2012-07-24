using System;

namespace BrewBuddy.Entities
{
    public class TemperatureAggregate
        : IEntity
    {
        public int Id { get; set; }
        public int BrewId { get; set; }
        public DateTime When { get; set; }
        public double Value { get; set; }
    }
}