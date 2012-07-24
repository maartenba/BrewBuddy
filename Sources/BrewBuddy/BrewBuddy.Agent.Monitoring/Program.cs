using System;
using System.Windows.Forms;
using BrewBuddy.Agent.Sensor.Properties;
using BrewBuddy.Sensor;
using BrewBuddy.Sensor.TemperUsb;

namespace BrewBuddy.Agent.Sensor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.SensorIdentifier))
            {
                Settings.Default.SensorIdentifier = Guid.NewGuid().ToString();
                Settings.Default.Save();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(
                new MainForm(
                    new DummySensor(),
                    new ServiceBusSensorApiV1(Settings.Default.SensorIdentifier)));

            // new TemperUsbTemperatureSensor()
        }
    }
}
