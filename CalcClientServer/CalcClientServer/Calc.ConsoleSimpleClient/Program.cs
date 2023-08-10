namespace Calc.ConsoleSimpleClient
{
    using System;
    using System.Configuration;
    using System.Net;

    using SocketClient;

    public class Program
    {
        public static void Main(string[] args)
        {
            var ipAddress = IPAddress.Parse(ConfigurationManager.AppSettings["IpAddress"]);
            var localEndPoint = new IPEndPoint(ipAddress, Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            /*using (var connection = new SynchronousCalcConnection<CalcRequest,CalcResponse>(localEndPoint, new CalcProtocolExecutor()))
            {
                long result = connection.DoOperation(Operation.Add(2, 3));

                Console.WriteLine("Result of 2 + 3 = {0}", result);
            }*/

            Console.ReadLine();
        }
    }
}
