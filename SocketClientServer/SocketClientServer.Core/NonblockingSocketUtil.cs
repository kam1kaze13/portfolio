using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientServer.Core
{
    public static class NonblockingSocketUtil
    {
        public static List<Socket> Accept(Socket listener)
        {
            var acceptedSockets = new List<Socket>();
            Socket socket = null;
            while (true)
            {
                // ждем 10000 мкс, не произошло ли что-то в сокете
                if (listener.Poll(10000, SelectMode.SelectRead))
                {
                    // если произошло
                    do
                    {
                        // получаем сокеты соединения с клиентами, пока они есть
                        try
                        {
                            socket = null;
                            socket = listener.Accept();
                            acceptedSockets.Add(socket);
                        }
                        catch (SocketException e)
                        {
                            // это нормальное поведение, когда Accept для неблокирующегогся сокета выкидывает такое исключение,
                            //  если нет соединений
                            if (e.SocketErrorCode != SocketError.WouldBlock)
                                throw;
                        }
                    }
                    while (socket != null);

                    // если есть соединения, возвращаем сокеты для них
                    if (acceptedSockets.Count > 0)
                        break;
                }
            }

            return acceptedSockets;
        }

        public static int Receive(Socket socket, byte[] buffer, int offset, int length, SocketFlags socketFlags, TimeSpan timeout)
        {
            int received = 0;
            DateTime startTime = DateTime.Now;
            while (true)
            {
                // ждем 10000 мкс, не произошло ли что-то в сокете
                if (socket.Poll(10000, SelectMode.SelectRead))
                {
                    // если произошло
                    try
                    {
                        // пробуем читать данные
                        received = socket.Receive(buffer, offset, length, socketFlags);
                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode != SocketError.WouldBlock)
                            throw;
                    }
                }

                // если данные были, то возвращаем их
                if (received > 0)
                    break;

                if (DateTime.Now - startTime >= timeout)
                    break;
            }

            return received;
        }
    }
}
