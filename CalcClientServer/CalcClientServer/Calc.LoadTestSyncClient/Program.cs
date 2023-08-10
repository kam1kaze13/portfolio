namespace Calc.LoadTest
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;

    using LoadTestUtil;
    using Protocol;
    using SocketClient;

    public static class Program
    {
        private static int startedOperations = 0;
        private static int successfulOperations = 0;
        private static int failedOperations = 0;

        private static bool isFinished = false;

        private static Random random = new Random();

        public static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(200, 200);
            ThreadPool.SetMinThreads(200, 200);

            var eventGenerator = new TimelyEventGenerator(Convert.ToDouble(ConfigurationManager.AppSettings["RequestPerSecond"]));

            eventGenerator.TimelyEvent +=
                () =>
                {
                    Task.Factory.StartNew(DoOperation);
                };

            Console.WriteLine("Sync client, load testing started...");

            eventGenerator.Start();

            Thread.Sleep(TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["TestingTime"])));

            eventGenerator.Stop();
            isFinished = true;

            Console.WriteLine("Total connections: {0}, {1} successful / {2} failed", startedOperations, successfulOperations, failedOperations);

            Console.ReadLine();
        }

        private static void DoOperation()
        {
            try
            {
                int arg1, arg2;
                OperationType op;
                lock (random)
                {
                    arg1 = random.Next(int.MinValue, int.MaxValue);
                    arg2 = random.Next(int.MinValue, int.MaxValue);
                    op = (OperationType) random.Next(0, 2);
                }

                Interlocked.Increment(ref startedOperations);

                using (var connection = CreateCalcConnection(
                    Convert.ToInt32(ConfigurationManager.AppSettings["Port"]),
                    ConfigurationManager.AppSettings["IpAddress"]))
                {
                    var response = connection.DoOperation(new CalcRequest { Argument1 =arg1, OperationType = op, Argument2 = arg2});
                    long actual = response.Result;
                    long expected;
                    switch (op)
                    {
                        case OperationType.Add:
                            expected = (long) arg1 + arg2;
                            break;
                        case OperationType.Sub:
                            expected = (long) arg1 - arg2;
                            break;
                        case OperationType.Mult:
                            expected = (long) arg1 * arg2;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!isFinished)
                    {
                        if (expected == actual)
                        {
                            Interlocked.Increment(ref successfulOperations);
                        }
                        else
                        {
                            Interlocked.Increment(ref failedOperations);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!isFinished)
                {
                    Interlocked.Increment(ref failedOperations);
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static SynchronousCalcConnection<CalcRequest, CalcResponse> CreateCalcConnection(int port, string ipAddressStr)
        {
            var ipAddress = IPAddress.Parse(ipAddressStr);
            var localEndPoint = new IPEndPoint(ipAddress, port);

            return new SynchronousCalcConnection<CalcRequest, CalcResponse>(localEndPoint, new CalcProtocolExecutor());
        }
    }
}
