namespace Calc.ConsoleSyncServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.Configuration;
    using SocketServer;
    using Calc.Protocol;

    public class Program
    {
        static void Main(string[] args)
        {
            var calcProcessor = new CalcProcessor();
            var server = new SynchronousCalcServer<CalcRequest, CalcResponse>(
                Convert.ToInt32(ConfigurationManager.AppSettings["Port"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["ListenerCount"]),
                new CalcProtocolExecutor(),
                calcProcessor.Process);
            try
            {
                server.Start();

                Console.WriteLine("Sync calc server started...");

                Console.ReadLine();
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
