using System;
using System.Data.Services.Client;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BrewBuddy.Sensor;
using FluentACS.ManagementService;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace BrewBuddy.Agent.Sensor
{
    public class ServiceBusSensorApiV1
        : ISensorApi
    {
        [DataContract(Name = "SensorReadingV1", Namespace = "urn:api.brewbuddy.net")]
        private class SensorReading
        {
            [DataMember]
            public string SensorId { get; set; }

            [DataMember]
            public DateTime When { get; set; }

            [DataMember]
            public double Value { get; set; }
        }

        private const string ServiceBusNamespace = "brewbuddy-euwest-1";
        private const string SensorApiTopic = "sensorapiv1";

        private string _issuerName;
        private string _issuerKey;

        private TopicClient _topicClient;

        public string SensorId { get; private set; }
        public bool IsLinkedToBrew { get; private set; }

        public ServiceBusSensorApiV1(string sensorId)
        {
            SensorId = sensorId;

            _issuerName = SensorId;
            _issuerKey = SensorId;
        }

        private void InitializeMessageSender()
        {
            var tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(_issuerName, Convert.ToBase64String(Encoding.UTF8.GetBytes(_issuerKey)));
            var serviceUrl = ServiceBusEnvironment.CreateServiceUri("sb", ServiceBusNamespace, string.Empty);

            var messagingFactory = MessagingFactory.Create(serviceUrl, tokenProvider);
            _topicClient = messagingFactory.CreateTopicClient(SensorApiTopic);
        }

        public void PrepareCommitBatch()
        {
            InitializeMessageSender();
        }

        public bool Commit(DateTime when, double value)
        {
            try
            {
                _topicClient.Send(new BrokeredMessage(new SensorReading { SensorId = SensorId, When = when, Value = value }));
                IsLinkedToBrew = true;
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                IsLinkedToBrew = false;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}