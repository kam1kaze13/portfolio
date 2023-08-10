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
            var eventGenerator = new TimelyEventGenerator(Convert.ToDouble(ConfigurationManager.AppSettings["RequestPerSecond"]));

            eventGenerator.TimelyEvent += StartDoOperation;

            Console.WriteLine("Async client, load testing started...");

            eventGenerator.Start();

            Thread.Sleep(TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["TestingTime"])));

            eventGenerator.Stop();
            isFinished = true;

            Console.WriteLine("Total connections: {0}, {1} successful / {2} failed", startedOperations, successfulOperations, failedOperations);

            Console.ReadLine();
        }

        private static void StartDoOperation()
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

            var ipAddress = IPAddress.Parse(ConfigurationManager.AppSettings["IpAddress"]);
            var localEndPoint = new IPEndPoint(ipAddress, Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));

            var asyncClient = new AsynchronousCalcClient();

            var disconnectCallback = new AsyncCallback(
                ar =>
                    {
                        try
                        {
                            asyncClient.EndDisconnect(ar);
                        }
                        catch (Exception e)
                        {
                            if (!isFinished)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    });

            AsyncCallback doOperationCallback = null;
            doOperationCallback = new AsyncCallback(
                ar =>
                    {
                        try
                        {
                            long actual = asyncClient.EndDoOperation(ar);
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

                            asyncClient.BeginDisconnect(disconnectCallback, null);
                        }
                        catch (Exception e)
                        {
                            if (!isFinished)
                            {
                                Interlocked.Increment(ref failedOperations);
                                Console.WriteLine(e.Message);
                            }
                        }
                    });

            var connectCallback = new AsyncCallback(
                ar =>
                    {
                        try
                        {
                            asyncClient.EndConnect(ar);

                            asyncClient.BeginDoOperation(
                                new Operation(arg1, op, arg2),
                                doOperationCallback,
                                null);
                        }
                        catch (Exception e)
                        {
                            if (!isFinished)
                            {
                                Interlocked.Increment(ref failedOperations);
                                Console.WriteLine(e.Message);
                            }
                        }
                    });


            try
            {
                asyncClient.BeginConnect(localEndPoint, connectCallback, null);
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
    }
}
