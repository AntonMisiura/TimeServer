using System;

namespace TimeServer.Impl.Command
{
    public class RoadSpeedCommand : CommandBase
    {
        public RoadSpeedCommand()
        {
            Name = "Road Speed";
            Pid = "01 0D";
            BytesNum = 1;
            RequestsNum = 1;
        }

        public int RoadSpeed { get; private set; }

        public override string ToString()
        {
            return $"{Name}: {RoadSpeed}";
        }

        protected override bool Parse(string data)
        {
            RoadSpeed = Convert.ToInt32(data.Split(' ')[2], 16);
            return true;
        }
    }
}