using System;
using System.IO.Ports;
using Microsoft.Extensions.Logging;
using TimeServer.Contract;

namespace TimeServer.Impl.Connection
{
    public class OdbSerialPortConnection : IOdbConnection
    {
        private OdbSerialPortSettings Config => OdbSerialPortSettings.Instance;
        private SerialPort Port = null;
        private ILogger _logger;

        public OdbSerialPortConnection(ILogger logger)
        {
            _logger = logger;
        }

        public OdbSerialPortConnection()
        {
            
        }

        public bool Open()
        {
            try
            {
                if (Port == null)
                {
                    Port = new SerialPort(Config.PortName, Config.BaudRate, Config.Parity, Config.DataBits, Config.StopBits);
                    Port.Open();
                    Console.WriteLine("Serial port is opened.");
                }

                return Port.IsOpen;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to open port: {ex.Message}");
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (Port != null)
                {
                    Port.Close();
                    Port = null;
                    Console.WriteLine("Serial port is closed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to close port: {ex.Message}");
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            Console.WriteLine("Reading data...");
            return Port?.Read(buffer, offset, count) ?? -1;
        }

        public void Write(string data)
        {
            Console.WriteLine("Writing data...");
            Port?.Write(data);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
