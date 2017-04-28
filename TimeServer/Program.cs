using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TimeServer
{
    internal class Program
    {
        private static byte[] _buffer = new byte[1024];
        private static string _engineRPM = string.Empty;
        private static string _roadSpeed = string.Empty;
        private static string _throttlePosition = string.Empty;
        private static string _engineTemperature = string.Empty;
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static int BaudRate = 9600;
        private static string PortName = "COM9";
        private static SerialPort sp = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);

        private static void Main(string[] args)
        {
            Console.Title = "Server";
            OpenSerialPort();
            Console.WriteLine("Please choose some data on the OBD simulator and push some button to accept...");
            Console.ReadKey();
            TimerOBDDataTick();
            SetupServer();
            Console.ReadLine();
        }

        private static void OpenSerialPort()
        {
            sp.PortName = PortName;
            sp.BaudRate = BaudRate;
            sp.Open();

            if (sp.IsOpen)
            {
                sp.ReadTimeout = 200;

                //discard any stuff in the buffers
                sp.DiscardOutBuffer();
                sp.DiscardInBuffer();
                Console.WriteLine("Serial port is opened...");
            }
        }

        private static void SetupServer()
        {
            Console.WriteLine("setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 35000));
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            var socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(socket);
            Console.WriteLine("Client connected");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {

            Socket socket = (Socket) ar.AsyncState;

            try
            {
                int received = socket.EndReceive(ar);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);

                string text = Encoding.ASCII.GetString(dataBuf);
                Console.WriteLine("Text received: " + text);
                string obdDataText = DateTime.Now.ToString() + "\n" + "\n" + "OBDSIM data: " + "\n" + "\n" + 
                    "Engine RPM: " + _engineRPM + "\n" + "\n" +
                    "Engine Temperature: " + _engineTemperature + "\n" + "\n" +
                    "Road Speed: " + _roadSpeed + "\n" + "\n" +
                    "Throttle Position: " + _throttlePosition + "\n";

                if (text.ToLower() == "get data")
                {            
                    SendData(obdDataText, socket);
                }
                else
                {
                    SendData("invalid request", socket);
                }
            }
            catch
            {
                Console.WriteLine("client disconnected");
                socket.Close();
                socket.Dispose();
            }
        }

        private static void SendData(string toString, Socket socket)
        {
            var data = Encoding.ASCII.GetBytes(toString);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        private static string getData(string pid, string numRequests, int numBytes)
        {
            int buffSize = 20;                      //holds the streamed data
            int iPid = 0;                           //occurence of pid in string
            int count = 0;                          //how many bytes sent
            byte[] pidBuff = new byte[buffSize];    // the pid data buffer
            string retVal = string.Empty;           //string to be returned

            //write pid requested
            sp.Write(pid + numRequests + "\r");
            
            //replace the pid request with 41 to match the OBD response
            pid = pid.Replace("01", "41");

            bool isContinue = true;
            iPid = 0;

            while (isContinue)
            {
                //loop until a ">" is recd. End of line char with the ELM327
                //or 200 msec timeout and no read, exit the loop and timer function too
                try { count = sp.Read(pidBuff, 0, buffSize); }
                catch (TimeoutException) { return (retVal = "Time"); }
                
                retVal += System.Text.Encoding.Default.GetString(pidBuff, 0, count);

                if (retVal.Contains(">"))
                {
                    isContinue = false;
                }
            }

            //Get the data and put in array  and convert to a string and return string          
            iPid = retVal.IndexOf(pid);

            if ((iPid == -1) || (retVal.Contains("DATA")) || (retVal.Length < 7))
            {
                return (retVal = "-1");
            }

            retVal = retVal.Substring(iPid, (5 + 3 * numBytes));
            return (retVal);
        }
        
        private static void TimerOBDDataTick()
        {
            string pidData = string.Empty;

            // **** Engine RPM - 0C, request 1 reply, 2 bytes data
            pidData = getData("01 0C", " 1", 2);

            if (pidData != "-1")
            {
                if (pidData == "Time") return;
                //Engine RPM 
                int dataA = (int)Convert.ToInt32(pidData.Split(' ')[2], 16) * 256;
                int dataB = (int)Convert.ToInt32(pidData.Split(' ')[3], 16);

                int engineRPM = (dataA + dataB) / 4;
                var returnedEngineRPM = (Convert.ToString(engineRPM));

                _engineRPM = returnedEngineRPM;
                Console.WriteLine("PID Data: " + pidData + " \t");
                Console.WriteLine("Engine RPM: " + returnedEngineRPM);
            }


            // *** Engine coolant, request 1 reply, 1 byte of data
            pidData = getData("01 05", " 1", 1);

            if (pidData != "-1")
            {
                if (pidData == "Time") return;
                //Engine coolant
                int coolant = (int)Convert.ToInt32(pidData.Split(' ')[2], 16) - 40;
                var pBarEngineTemp = (Convert.ToString(coolant + 40));

                _engineTemperature = pBarEngineTemp;
                Console.WriteLine("EngineTemperature: " + pBarEngineTemp);
            }

            // **** Road Speed, request 1 reply, 1 byte of data
            pidData = getData("01 0D", " 1", 1);

            if (pidData != "-1")
            {
                if (pidData == "Time") return;
                //RoadSpeed Speed
                int roadSpeed = (int)Convert.ToInt32(pidData.Split(' ')[2], 16);
                var pBarRoadSpeed = (Convert.ToString(roadSpeed));

                _roadSpeed = pBarRoadSpeed;
                Console.WriteLine("Road Speed: " + pBarRoadSpeed);
            }

            //Throttle Position, request 1 reply, 1 byte Data
            pidData = getData("01 11", "1", 1);

            if (pidData != "-1")
            {
                if (pidData == "Time") return;
                int throttlePos = (int)Convert.ToInt32(pidData.Split(' ')[2], 16) * 100;
                var pBarThrottlePosition = (Convert.ToString((throttlePos) / 255));

                _throttlePosition = pBarThrottlePosition;
                Console.WriteLine("ThrottlePosition: " + pBarThrottlePosition);
            }
        }
    }
}
