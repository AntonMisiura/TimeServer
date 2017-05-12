using System;

namespace TimeServer.Impl.Command
{
    public class ThrottlePositionCommand : CommandBase
    {
        public ThrottlePositionCommand()
        {
            Name = "Throttle Position";
            Pid = "01 11";
            BytesNum = 1;
            RequestsNum = 1;
        }

        public int ThrottlePosition { get; private set; }

        public override string ToString()
        {
            return $"{Name}: {ThrottlePosition}";
        }

        protected override bool Parse(string data)
        {
            ThrottlePosition = Convert.ToInt32(data.Split(' ')[2], 16) * 100 / 255;
            return true;
        }
    }
}