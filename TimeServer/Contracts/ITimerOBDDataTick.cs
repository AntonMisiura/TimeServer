namespace TimeServer.Contracts
{
    public interface ITimerOBDDataTick
    {
        void GetEngineRPMData();

        void GetRoadSpeedData();

        void GetEngineTemperatureData();
        
        void GetThrottlePositionData();
    }
}
