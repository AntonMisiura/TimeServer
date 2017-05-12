
using System;

namespace TimeServer.Contract
{
    /// <summary>
    /// Represents connection to ODB device
    /// </summary>
    public interface IOdbConnection : IDisposable
    {
        bool Open();
        void Close();

        int Read(byte[] buffer, int offset, int count);
        void Write(string data);
    }
}
