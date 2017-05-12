using System;

namespace TimeServer.Impl.Command
{
    public class EngineRpmCommand : CommandBase
    {
        public EngineRpmCommand()
        {
            Name = "Engine RPM";
            Pid = "01 0C";
            BytesNum = 2;
            RequestsNum = 1;
        }

        public double EngineRpm { get; private set; }

        public override string ToString()
        {
            return $"{Name}: {EngineRpm}";
        }

        protected override bool Parse(string data)
        {
            var dataA = Convert.ToInt32(data.Split(' ')[2], 16) * 256;
            var dataB = Convert.ToInt32(data.Split(' ')[3], 16);

            EngineRpm = (dataA + dataB) / 4.0;
            return true;
        }
    }
}