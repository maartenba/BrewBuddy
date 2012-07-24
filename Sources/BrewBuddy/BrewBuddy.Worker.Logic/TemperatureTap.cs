using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using BrewBuddy.Entities;
using BrewBuddy.Services;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace BrewBuddy.Worker.Logic
{
    public class TemperatureTap
        : IWorker
    {
        private SubscriptionClient _subscriptionClient;

        private const string ServiceBusNamespace = "brewbuddy-euwest-1";
        private const string SensorApiTopic = "sensorapiv1";
        private const string OwnerIssuer = "owner";
        private const string OwnerKey = "your key";

        public void Run()
        {
            InitializeSubscriptionClient();

            var entitiesContext = new EntitiesContext();
            ITemperatureAggregateService temperatureAggregateService = new TemperatureAggregateService(
                new EntityRepository<TemperatureAggregate>(entitiesContext), new EntityRepository<Brew>(entitiesContext));

            while (true)
            {
                Trace.WriteLine("Receiving message...", "Info");
                var brokeredMessage = _subscriptionClient.Receive();
                if (brokeredMessage != null)
                {
                    Trace.WriteLine("Received message.", "Info");
                    Trace.WriteLine(
                        string.Format("Message id: {0} - Message enqueued: {1}", brokeredMessage.MessageId,
                                      brokeredMessage.EnqueuedTimeUtc), "Verbose");

                    SensorReading sensorReading = null;

                    // We support two message types: a serialized SensorReading and one that only sends us
                    // message properties (for netmf support). Let's see what we have...
                    if (brokeredMessage.Properties.Any() && brokeredMessage.Properties.ContainsKey("SensorId"))
                    {
                        sensorReading = new SensorReading
                                            {
                                                SensorId = (string)brokeredMessage.Properties["SensorId"],
                                                When = new DateTime(Convert.ToInt64(brokeredMessage.Properties["When"]), DateTimeKind.Utc),
                                                Value = Convert.ToDouble(brokeredMessage.Properties["Value"], CultureInfo.CreateSpecificCulture("en-US"))
                                            };
                        if (sensorReading.When.Year > (DateTime.UtcNow.Year + 1) || sensorReading.When.Year < DateTime.UtcNow.Year)
                        {
                            sensorReading.When = brokeredMessage.EnqueuedTimeUtc;
                        }
                    }
                    else
                    {
                        sensorReading = brokeredMessage.GetBody<SensorReading>();
                    }

                    Trace.WriteLine(string.Format("Message id: {0} - Sensor: {1} - Sensor reading: {2}", brokeredMessage.MessageId, sensorReading.SensorId, sensorReading.Value), "Verbose");

                    Trace.WriteLine("Processing message...", "Info");
                    temperatureAggregateService.AggregateData(sensorReading.SensorId, sensorReading.When, sensorReading.Value);

                    brokeredMessage.Complete();
                    Trace.WriteLine("Processed message.", "Info");
                }
            }
        }

        private void InitializeSubscriptionClient()
        {
            var tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(OwnerIssuer, OwnerKey);
            var serviceUrl = ServiceBusEnvironment.CreateServiceUri("sb", ServiceBusNamespace, string.Empty);

            var namespaceManager = new NamespaceManager(serviceUrl, tokenProvider);

            Trace.WriteLine("Initializing Subscription...", "Info");
            SubscriptionDescription subscriptionDescription = new SubscriptionDescription(SensorApiTopic, "TemperatureTap");
            if (!namespaceManager.SubscriptionExists(SensorApiTopic, "TemperatureTap"))
            {
                namespaceManager.CreateSubscription(subscriptionDescription);
            }
            Trace.WriteLine("Initialized Subscription.", "Info");

            Trace.WriteLine("Initializing SubscriptionClient...", "Info");
            var messagingFactory = MessagingFactory.Create(serviceUrl, tokenProvider);
            _subscriptionClient = messagingFactory.CreateSubscriptionClient(SensorApiTopic, "TemperatureTap");
            Trace.WriteLine("Initialized SubscriptionClient...", "Info");
        }
    }
}