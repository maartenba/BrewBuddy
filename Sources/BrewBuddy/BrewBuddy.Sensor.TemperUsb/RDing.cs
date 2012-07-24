using System;
using System.Runtime.InteropServices;

namespace BrewBuddy.Sensor.TemperUsb
{
    internal class RDing
    {
        [DllImport("RDingUSB.dll")]
        public static extern IntPtr OpenUSBDevice(int VID, int PID);

        [DllImport("RDingUSB.dll")]
        public static extern IntPtr CloseUSBDevice(IntPtr hDevice);

        [DllImport("RDingUSB.dll")]
        public static extern bool WriteUSB(IntPtr hDevice, byte[] pBuffer, uint dwBytesToWrite,
                                           ref ulong lpNumberOfBytesWritten);

        [DllImport("RDingUSB.dll")]
        public static extern bool ReadUSB(IntPtr hDevice, byte[] pBuffer, uint dwBytesToRead,
                                          ref ulong lpNumberOfBytesRead);

        [DllImport("RDingUSB.dll")]
        public static extern ushort GetInputLength(IntPtr hDevice);

        [DllImport("RDingUSB.dll")]
        public static extern ushort GetOutputLength(IntPtr hDevice);

        [DllImport("RDingUSB.dll")]
        public static extern uint GetErrorMsg(ref string[] lpErrorMsg, uint dwErrorMsgSize);
    }
}