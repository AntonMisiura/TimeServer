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
        private static byte [] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static string PortName = "COM9";
        private static int BaudRate = 9600;

        private static SerialPort sp = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);

        private static void Main(string[] args)
        {
            Console.Title = "Server";
            OpenSerialPort();
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

                if (text.ToLower() == "get time")
                {
                    SendData(DateTime.Now.ToString(), socket);
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
