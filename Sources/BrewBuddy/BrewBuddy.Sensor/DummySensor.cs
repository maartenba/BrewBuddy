using System;

namespace BrewBuddy.Sensor
{
    public class DummySensor
        : ISensor
    {
        private Random _random;

        public DummySensor()
        {
            _random = new Random((int)DateTime.UtcNow.Ticks);
        }

        public double? Read()
        {
            return Math.Round((double)_random.Next(22000, 24000) / 1000, 3);
        }
    }
}