using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zad4
{

    class Program
    {
        private static Object thisLock = new Object();
        static void writeConsoleMessage(string message, ConsoleColor color)
        {
            lock (thisLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }

        }


        static void ThreadProc(Object stateInfo)
        {
            var typ = ((object[])stateInfo)[0];

            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc1, new object[] { 'z', client });

            }

        }
        static void ThreadProc1(Object stateInfo)
        {
            var typ = ((object[])stateInfo)[0];
            if ((char)typ == 'c')
            {

                TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
                NetworkStream stream = client.GetStream();
                byte[] message = new ASCIIEncoding().GetBytes("InzynieriaOprogramowania");
                writeConsoleMessage("Klient: wysylam wiadomosc", ConsoleColor.Green);
                client.GetStream().Write(message, 0, message.Length);

            }
            if ((char)typ == 'z')
            {

                TcpClient client = (TcpClient)((object[])stateInfo)[1];
                byte[] buffer = new byte[1024];
                int n = client.GetStream().Read(buffer, 0, 1024);
                client.GetStream().Write(buffer, 0, buffer.Length);
                string msgfromclient = new ASCIIEncoding().GetString(buffer, 0, n);

                writeConsoleMessage("Serwer: otrzymalem i odsylam wiadomosc: " + msgfromclient, ConsoleColor.Red);

                client.Close();
            }

        }
        static void Main(string[] args)
        {

            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] { 's' });
            ThreadPool.QueueUserWorkItem(ThreadProc1, new object[] { 'c' });
            ThreadPool.QueueUserWorkItem(ThreadProc1, new object[] { 'c' });
            ThreadPool.QueueUserWorkItem(ThreadProc1, new object[] { 'c' });
            ThreadPool.QueueUserWorkItem(ThreadProc1, new object[] { 'c' });
            Console.ReadKey();
        }
    }
}

