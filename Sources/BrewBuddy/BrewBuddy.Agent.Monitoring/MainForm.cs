using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrewBuddy.Agent.Sensor.Properties;
using BrewBuddy.Sensor;

namespace BrewBuddy.Agent.Sensor
{
    public partial class MainForm : Form
    {
        private readonly ISensor _sensor;
        private readonly ISensorApi _sensorApi;

        public string Temperature { get; set; }
        public string LastSynced { get; set; }
        public string SensorId { get; set; }

        private Dictionary<DateTime, double?> readings = new Dictionary<DateTime, double?>();

        public MainForm(ISensor sensor, ISensorApi sensorApi)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.SensorIdentifier))
            {
                Settings.Default.SensorIdentifier = Guid.NewGuid().ToString();
                Settings.Default.Save();
            }

            _sensor = sensor;
            _sensorApi = sensorApi;

            LastSynced = "(unknown)";
            SensorId = Settings.Default.SensorIdentifier;

            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Left = 9999999;
        }

        private void UpdateTemperature()
        {
            var value = _sensor.Read();
            if (value.HasValue)
            {
                Temperature = string.Format("{0:0.00} °C", value);
            }
            else
            {
                Temperature = "(unknown)";
            }
            if (!readings.ContainsKey(DateTime.Now))
            {
                readings.Add(DateTime.Now, value);
            }
        }

        private void UploadData()
        {
            if (readings.Any())
            {
                _sensorApi.PrepareCommitBatch();

                List<DateTime> itemsToRemove = new List<DateTime>();
                foreach (var reading in readings)
                {
                    if (_sensorApi.Commit(reading.Key, reading.Value ?? 0))
                    {
                        itemsToRemove.Add(reading.Key);
                    }
                }
                foreach (var itemToRemove in itemsToRemove)
                {
                    readings.Remove(itemToRemove);
                }

                LastSynced = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateTemperature();
        }

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            UploadData();
        }

        private void copyToClipboardMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SensorId);
        }

        private void quitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                string linkedInfo = "Linked to BrewBuddy.net";
                if (!_sensorApi.IsLinkedToBrew)
                {
                    linkedInfo = "Not linked to BrewBuddy.net. Copy the sensor ID by right-clicking the BrewBuddy icon, navigate to a brew on BrewBuddy.net and link this sensor to get monitoring data.";
                }

                notifyIcon.ShowBalloonTip(3000, "BrewBuddy",
                    string.Format("Temperature: {0}\r\nSynchronized: {1}\r\nSensor ID: {2}\r\n\r\n{3}", Temperature, LastSynced, SensorId, linkedInfo), ToolTipIcon.Info);
            }
        }
    }
}
