using System.IO.Ports;

namespace TimeServer.Repo
{
    public abstract class BaseConst
    {
        public BaseConst()
        {

        }

        protected int IPid = 0;                           //occurence of pid in string
        protected int Count = 0;                          //how many bytes sent
        protected int BuffSize = 20;                      //holds the streamed data
        protected string RetVal = string.Empty;           //string to be returned
        protected byte[] PidBuff = new byte[20];          // the pid data buffer

        protected string PIDData = string.Empty;
        protected static string EngineRPM = string.Empty;
        protected static string RoadSpeed = string.Empty;
        protected static string ThrottlePosition = string.Empty;
        protected static string EngineTemperature = string.Empty;

        protected static int BaudRate = 9600;
        protected static string PortName = "COM9";
        protected static SerialPort SerialPort = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
    }
}
