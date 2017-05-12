namespace TimeServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var manager = new OdbManager();
            manager.Execute();
        }
    }
}
