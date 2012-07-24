using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using BrewBuddy.Sensor;

namespace BrewBuddy.Agent.Sensor
{
    public class WebApiSensorApiV1
        : ISensorApi
    {
        private const string ServiceUrl = "http://localhost:1460/api/v1/sensor";

        public string SensorId { get; private set; }
        public bool IsLinkedToBrew
        {
            get { return true; }
        }

        public WebApiSensorApiV1(string sensorId)
        {
            SensorId = sensorId;
        }

        public void PrepareCommitBatch()
        {
            // noop
        }

        public bool Commit(DateTime when, double value)
        {
            using (HttpClient client = new HttpClient())
            {
                var form = new Dictionary<string, string>();
                form.Add("sensorId", SensorId);
                form.Add("eventDate", when.ToString());
                form.Add("sensorValue", value.ToString());
                var content = new FormUrlEncodedContent(form);

                var result = client.PutAsync(ServiceUrl, content);
                result.Wait();
                return result.Result.StatusCode == HttpStatusCode.Created;
            }
        }
    }
}