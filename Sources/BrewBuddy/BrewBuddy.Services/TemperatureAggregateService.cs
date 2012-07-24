using System;
using System.Collections.Generic;
using System.Linq;
using BrewBuddy.Entities;

namespace BrewBuddy.Services
{
    public class TemperatureAggregateService
        : ITemperatureAggregateService
    {
        protected IEntityRepository<TemperatureAggregate> TemperatureAggregateRepository { get; private set; }
        protected IEntityRepository<Brew> BrewRepository { get; private set; }

        public TemperatureAggregateService(IEntityRepository<TemperatureAggregate> temperatureAggregateRepository, IEntityRepository<Brew> brewRepository)
        {
            TemperatureAggregateRepository = temperatureAggregateRepository;
            BrewRepository = brewRepository;
        }

        public IEnumerable<TemperatureAggregate> GetForBrew(int brewId)
        {
            return TemperatureAggregateRepository.GetAll().Where(a => a.BrewId == brewId).OrderBy(a => a.When);
        }

        public IEnumerable<TemperatureAggregate> GetForBrew(int brewId, DateTime @from, DateTime to)
        {
            var lowerBoundary = @from.Round(TimeSpan.FromMinutes(10));
            var upperBoundary = to.Round(TimeSpan.FromMinutes(10));

            return TemperatureAggregateRepository.GetAll().Where(a => a.BrewId == brewId && a.When >= lowerBoundary && a.When <= upperBoundary).OrderBy(a => a.When);
        }

        public void AggregateData(string sensorId, DateTime when, double value)
        {
            // Find linked brew
            var brew = BrewRepository.GetAll().FirstOrDefault(b => b.SensorId == sensorId);
            if (brew != null)
            {
                // Find previous aggregate
                var brewId = brew.Id;
                var lowerBoundary = when.Round(TimeSpan.FromMinutes(10));
                var upperBoundary = lowerBoundary.AddMinutes(1);

                var aggregate = TemperatureAggregateRepository.GetAll().FirstOrDefault(a => a.BrewId == brewId && a.When >= lowerBoundary && a.When <= upperBoundary);
                if (aggregate == null)
                {
                    TemperatureAggregateRepository.InsertOnCommit(new TemperatureAggregate { BrewId = brewId, Value = value, When = lowerBoundary });
                }
                else
                {
                    aggregate.Value = (aggregate.Value + value) / 2;
                }
                TemperatureAggregateRepository.CommitChanges();
            }
        }
    }
}