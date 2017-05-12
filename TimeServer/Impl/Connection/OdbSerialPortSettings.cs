
using System.Configuration;
using System.IO.Ports;

namespace TimeServer.Impl.Connection
{
    /// <summary>
    ///
    /// </summary>
    public class OdbSerialPortSettings : ConfigurationSection
    {
        /// <summary>
        /// Instance of the settings
        /// </summary>
        public static OdbSerialPortSettings Instance => ConfigurationManager.GetSection("ODB.Connection.SerialPort") as OdbSerialPortSettings;

        [ConfigurationProperty(nameof(PortName), IsRequired = true)]
        public string PortName
        {
            get { return (string) this[nameof(PortName)]; }
            set { this[nameof(PortName)] = value; }
        }

        [ConfigurationProperty(nameof(BaudRate), DefaultValue = 9600)]
        public int BaudRate
        {
            get { return (int)this[nameof(BaudRate)]; }
            set { this[nameof(BaudRate)] = value; }
        }

        [ConfigurationProperty(nameof(Parity), DefaultValue = Parity.None)]
        public Parity Parity
        {
            get { return (Parity)this[nameof(Parity)]; }
            set { this[nameof(Parity)] = value; }
        }

        [ConfigurationProperty(nameof(DataBits), DefaultValue = 8)]
        public int DataBits
        {
            get { return (int)this[nameof(DataBits)]; }
            set { this[nameof(DataBits)] = value; }
        }

        [ConfigurationProperty(nameof(StopBits), DefaultValue = StopBits.One)]
        public StopBits StopBits
        {
            get { return (StopBits)this[nameof(StopBits)]; }
            set { this[nameof(StopBits)] = value; }
        }
    }
}