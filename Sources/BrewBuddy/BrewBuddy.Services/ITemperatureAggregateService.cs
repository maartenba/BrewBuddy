using System;
using System.Collections.Generic;
using BrewBuddy.Entities;

namespace BrewBuddy.Services
{
    public interface ITemperatureAggregateService
    {
        IEnumerable<TemperatureAggregate> GetForBrew(int brewId);
        IEnumerable<TemperatureAggregate> GetForBrew(int brewId, DateTime from, DateTime to);
        void AggregateData(string sensorId, DateTime when, double value);
    }
}