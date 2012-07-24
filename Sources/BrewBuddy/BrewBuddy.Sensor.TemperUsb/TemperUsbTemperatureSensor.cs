using System;
using System.Text;
using System.Threading;

namespace BrewBuddy.Sensor.TemperUsb
{
    public sealed class TemperUsbTemperatureSensor
        : ISensor, IDisposable
    {
        private const int UsbVendorId = 3141;
        private const int UsbDeviceId = 29697;

        private readonly byte[] _readTemperatureCommand = new byte[] { 0, 1, 128, 51, 1, 0, 0, 0, 0 };
        private readonly byte[] _readCalibrationCommand = new byte[] { 0, 1, 130, 119, 1, 0, 0, 0, 0 };
        private readonly byte[] _readVersionCommand = new byte[] { 0, 1, 134, 255, 1, 0, 0, 0, 0 };

        private IntPtr _device = new IntPtr(-1);
        private int _deviceType;
        ushort _deviceOutputLength;
        ushort _deviceInputLength;
        ulong _numberOfBytesWritten = 0uL;
        ulong _numberOfBytesRead = 0uL;

        public TemperUsbTemperatureSensor()
        {
            Connect();
            InitializeDevice();
        }

        public double? Read()
        {
            byte[] transportBuffer = new byte[9];

            bool readSuccessful = false;
            if (RDing.WriteUSB(_device, _readTemperatureCommand, (uint)_deviceOutputLength, ref _numberOfBytesWritten))
            {
                Thread.Sleep(50);
                readSuccessful = RDing.ReadUSB(_device, transportBuffer, (uint)_deviceInputLength, ref _numberOfBytesRead);
            }

            if (!readSuccessful)
            {
                return null;
            }

            double temperatureCorrection;
            double temperature;

            if (transportBuffer[1] == 128)
            {
                RDing.WriteUSB(_device, _readCalibrationCommand, (uint)_deviceOutputLength, ref _numberOfBytesWritten);
                Thread.Sleep(100);
                byte[] calibrationBuffer = new byte[9];
                RDing.ReadUSB(_device, calibrationBuffer, (uint)_deviceInputLength, ref _numberOfBytesRead);
                if (calibrationBuffer[1] == 130)
                {
                    if (calibrationBuffer[3] <= 127)
                    {
                        temperatureCorrection = (double)calibrationBuffer[3] * 0.0625;
                    }
                    else
                    {
                        int test = 256 - (int)calibrationBuffer[3];
                        temperatureCorrection = (double)(-1 * test) * 0.0625;
                    }
                }
            }

            if (transportBuffer[3] > 128)
            {
                temperature = -1.0 * ((double)Convert.ToInt32(256 - (int)transportBuffer[3]) + (double)(transportBuffer[4] >> 4 & 15) * 0.0625);
                if (_deviceType == 1 || _deviceType == 3)
                {
                    temperature = (double)(-1 * Convert.ToInt32((double)(256 - (int)transportBuffer[5]) + (double)(transportBuffer[6] >> 4 & 15) * 0.0625));
                }
                return temperature;
            }
            else
            {
                temperature = (double)Convert.ToInt32(transportBuffer[3]) + (double)(transportBuffer[4] >> 4 & 15) * 0.0625;
                if (_deviceType == 1 || _deviceType == 3)
                {
                    temperature = (double)Convert.ToInt32(transportBuffer[5]) + (double)(transportBuffer[6] >> 4 & 15) * 0.0625;
                }
                return temperature;
            }
        }

        public void Dispose()
        {
            if (_device.ToInt32() != -1)
            {
                RDing.CloseUSBDevice(_device);
            }
        }

        private void Connect()
        {
            _device = RDing.OpenUSBDevice(UsbVendorId, UsbDeviceId);

            if (_device.ToInt32() == -1)
            {
                throw new Exception(
                    "Could not open TEMPer USB device. Make sure the device is connected and rivers are installed.");
            }
        }

        private void InitializeDevice()
        {
            // Initialize device communication variables
            _deviceOutputLength = RDing.GetOutputLength(_device);
            _deviceInputLength = RDing.GetInputLength(_device);

            // Retrieve device version
            byte[] bVersion = new byte[9];
            byte[] bVersion2 = new byte[9];

            RDing.WriteUSB(_device, _readVersionCommand, (uint)_deviceOutputLength, ref _numberOfBytesWritten);
            Thread.Sleep(100);
            RDing.ReadUSB(_device, bVersion, (uint)_deviceInputLength, ref _numberOfBytesRead);
            var versions = Encoding.ASCII.GetString(bVersion, 1, 8);
            Thread.Sleep(100);
            RDing.ReadUSB(_device, bVersion2, (uint)_deviceInputLength, ref _numberOfBytesRead);
            var str = Encoding.ASCII.GetString(bVersion2, 1, 8);

            for (int i = 0; i < versions.Length; i++)
            {
                if (versions[i] == str[i])
                {
                    break;
                }
                versions += str[i].ToString();
            }

            if (versions.Contains("TEMPer2"))
            {
                if (versions.Contains("1.1"))
                {
                    _deviceType = 3;
                }
                else
                {
                    _deviceType = 1;
                }
            }
            else
            {
                _deviceType = 2;
            }
        }
    }
}