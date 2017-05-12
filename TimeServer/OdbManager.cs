using System;
using TimeServer.Contract;
using TimeServer.Impl.Command;
using TimeServer.Impl.Connection;

namespace TimeServer
{
    public class OdbManager
    {
        private IOdbCommand[] _commands;
        private IOdbConnection _connection;

        public OdbManager()
        {
            // Load below code from configuration or inject
            _connection = new OdbSerialPortConnection();
            _commands = new IOdbCommand[]
            {
                new EngineRpmCommand(),
                new EngineTemperatureCommand(),
                new RoadSpeedCommand(),
                new ThrottlePositionCommand()
            };
        }

        public string Execute()
        {
            _connection.Open();

            var response = string.Empty;
            foreach (var command in _commands)
            {
                command.Execute(_connection);
                response += $"{command}\n";
                Console.ReadKey();
            }

            _connection.Close();

            return response;
        }
    }
}