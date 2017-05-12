
namespace TimeServer.Contract
{
    /// <summary>
    /// Represent ODB command or parameter of the car
    /// </summary>
    public interface IOdbCommand
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Convert all command data to string
        /// </summary>
        /// <returns> Converted command data to string </returns>
        string ToString();

        /// <summary>
        /// Executes command
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        bool Execute(IOdbConnection connection);
    }
}
