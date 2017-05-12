using System;
using System.Net;
using System.Text;
using System.IO.Ports;
using TimeServer.Repo;
using System.Net.Sockets;
using TimeServer.Contracts;
using System.Collections.Generic;


namespace TimeServer
{
    internal class ProgramMessy : BaseConst
    {
        private static IDataOperations _dataOperations;

        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public ProgramMessy(IDataOperations dataOperations)
        {
            _dataOperations = dataOperations;
        }

        private static void MainRetired(string[] args)
        {
            Console.Title = "Server";
            OpenSerialPort();
            Console.WriteLine("Please choose some data on the OBD simulator and push some button to accept...");
            Console.ReadKey();
            _dataOperations.ShowOBDData();
            SetupServer();
            Console.ReadLine();
        }

        private static void OpenSerialPort()
        {
            SerialPort.PortName = PortName;
            SerialPort.BaudRate = BaudRate;
            SerialPort.Open();

            if (SerialPort.IsOpen)
            {
                SerialPort.ReadTimeout = 200;

                //discard any stuff in the buffers
                SerialPort.DiscardOutBuffer();
                SerialPort.DiscardInBuffer();
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
                    "Engine RPM: " + EngineRPM + "\n" + "\n" +
                    "Engine Temperature: " + EngineTemperature + "\n" + "\n" +
                    "Road Speed: " + RoadSpeed + "\n" + "\n" +
                    "Throttle Position: " + ThrottlePosition + "\n";

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
    }
}
