namespace TimeServer.Contracts
{
    public interface IDataOperations
    {
        string getData(string pid, string numRequests, int numBytes);

        void ShowOBDData();
    }
}
