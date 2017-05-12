using System;
using System.IO.Ports;
using TimeServer.Contract;

namespace TimeServer.Impl.Connection
{
    class OdbSerialPortConnection : IOdbConnection
    {
        private OdbSerialPortSettings Config => OdbSerialPortSettings.Instance;
        private SerialPort Port = null;

        public bool Open()
        {
            try
            {
                if (Port == null)
                {
                    Port = new SerialPort(Config.PortName, Config.BaudRate, Config.Parity, Config.DataBits, Config.StopBits);
                    Port.Open();
                }

                return Port.IsOpen;
            }
            catch (Exception ex)
            {
                // Logging
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
                }
            }
            catch (Exception)
            {
                // Logging
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return Port?.Read(buffer, offset, count) ?? -1;
        }

        public void Write(string data)
        {
            Port?.Write(data);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
