using System;
using TimeServer.Contracts;

namespace TimeServer.Repo
{
    public class TimerOBDDataTick : BaseConst, ITimerOBDDataTick
    {
        private IDataOperations _dataOperations;

        public TimerOBDDataTick(IDataOperations dataOperations)
        {
            _dataOperations = dataOperations;
        }

        public void GetEngineRPMData()
        {
            // **** Engine RPM - 0C, request 1 reply, 2 bytes data
            PIDData = _dataOperations.getData("01 0C", " 1", 2);

            if (PIDData != "-1")
            {
                if (PIDData == "Time") return;
                //Engine RPM 
                int dataA = (int)Convert.ToInt32(PIDData.Split(' ')[2], 16) * 256;
                int dataB = (int)Convert.ToInt32(PIDData.Split(' ')[3], 16);

                int engineRPM = (dataA + dataB) / 4;
                var returnedEngineRPM = (Convert.ToString(engineRPM));

                EngineRPM = returnedEngineRPM;
                Console.WriteLine("PID Data: " + PIDData + " \t");
                Console.WriteLine("Engine RPM: " + returnedEngineRPM);
            }
        }

        public void GetEngineTemperatureData()
        {
            // *** Engine coolant, request 1 reply, 1 byte of data
            PIDData = _dataOperations.getData("01 05", " 1", 1);

            if (PIDData != "-1")
            {
                if (PIDData == "Time") return;
                //Engine coolant
                int coolant = (int)Convert.ToInt32(PIDData.Split(' ')[2], 16) - 40;
                var pBarEngineTemp = (Convert.ToString(coolant + 40));

                EngineTemperature = pBarEngineTemp;
                Console.WriteLine("EngineTemperature: " + pBarEngineTemp);
            }
        }

        public void GetRoadSpeedData()
        {
            // **** Road Speed, request 1 reply, 1 byte of data
            PIDData = _dataOperations.getData("01 0D", " 1", 1);

            if (PIDData != "-1")
            {
                if (PIDData == "Time") return;
                //RoadSpeed Speed
                int roadSpeed = (int)Convert.ToInt32(PIDData.Split(' ')[2], 16);
                var pBarRoadSpeed = (Convert.ToString(roadSpeed));

                RoadSpeed = pBarRoadSpeed;
                Console.WriteLine("Road Speed: " + pBarRoadSpeed);
            }
        }

        public void GetThrottlePositionData()
        {
            //Throttle Position, request 1 reply, 1 byte Data
            PIDData = _dataOperations.getData("01 11", "1", 1);

            if (PIDData != "-1")
            {
                if (PIDData == "Time") return;
                int throttlePos = (int)Convert.ToInt32(PIDData.Split(' ')[2], 16) * 100;
                var pBarThrottlePosition = (Convert.ToString((throttlePos) / 255));

                ThrottlePosition = pBarThrottlePosition;
                Console.WriteLine("ThrottlePosition: " + pBarThrottlePosition);
            }
        }
    }
}
