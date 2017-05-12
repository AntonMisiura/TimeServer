
using TimeServer.Contract;
using TimeServer.Impl.Command;
using TimeServer.Impl.Connection;

namespace TimeServer
{
    class OdbManager
    {
        private IOdbCommand[] commands;
        private IOdbConnection connection;

        public OdbManager()
        {
            // Load below code from configuration or inject
            connection = new OdbSerialPortConnection();
            commands = new IOdbCommand[]
            {
                new EngineRpmCommand(),
                new EngineTemperatureCommand(),
            };
        }

        public string Execute()
        {
            connection.Open();

            var response = string.Empty;
            foreach (var command in commands)
            {
                command.Execute(connection);
                response += $"{command}\n";
            }

            connection.Close();

            return response;
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            OdbManager manager = new OdbManager();
            var response = manager.Execute();

            return;
        }
    }
}
