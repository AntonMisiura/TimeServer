using System;
using System.IO.Ports;
using TimeServer.Contracts;

namespace TimeServer.Repo
{
    public class DataOperations: BaseConst, IDataOperations 
    {
        private ITimerOBDDataTick _timerOBDDataTick;

        public DataOperations(ITimerOBDDataTick timerOBDDataTick)
        {
            _timerOBDDataTick = timerOBDDataTick;
        }
        
        public string getData(string pid, string numRequests, int numBytes)
        {
            //write pid requested
            SerialPort.Write(pid + numRequests + "\r");

            //replace the pid request with 41 to match the OBD response
            pid = pid.Replace("01", "41");

            bool isContinue = true;
            IPid = 0;

            while (isContinue)
            {
                //loop until a ">" is recd. End of line char with the ELM327
                //or 200 msec timeout and no read, exit the loop and timer function too
                try { Count = SerialPort.Read(PidBuff, 0, BuffSize); }
                catch (TimeoutException) { return (RetVal = "Time"); }

                RetVal += System.Text.Encoding.Default.GetString(PidBuff, 0, Count);

                if (RetVal.Contains(">"))
                {
                    isContinue = false;
                }
            }

            //Get the data and put in array  and convert to a string and return string          
            IPid = RetVal.IndexOf(pid);

            if ((IPid == -1) || (RetVal.Contains("DATA")) || (RetVal.Length < 7))
            {
                return (RetVal = "-1");
            }

            RetVal = RetVal.Substring(IPid, (5 + 3 * numBytes));
            return (RetVal);
        }
             
        public void ShowOBDData()
        {
            _timerOBDDataTick.GetEngineRPMData();
            _timerOBDDataTick.GetRoadSpeedData();
            _timerOBDDataTick.GetThrottlePositionData();
            _timerOBDDataTick.GetEngineTemperatureData();
        }
    }
}
