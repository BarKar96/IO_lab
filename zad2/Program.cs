using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zad2
{

    class Program
    {
        static void ThreadProc(Object stateInfo)
        {
            var typ = ((object[])stateInfo)[0];
            if ((char)typ == 's')
            {
                TcpListener server = new TcpListener(IPAddress.Any, 2048);
                server.Start();
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    byte[] buffer = new byte[1024];
                    int n = client.GetStream().Read(buffer, 0, 1024);                    
                    string msgfromclient = new ASCIIEncoding().GetString(buffer, 0, n);
                    Console.WriteLine("Server: otrzymalem wiadomosc: " + msgfromclient);
                    client.GetStream().Write(buffer, 0, buffer.Length);
                    client.Close();
                }
            }
            if ((char)typ == 'c')
            {
                TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
                string wiadomosc = "TEST";
                Console.WriteLine("Klient: wysylam wiadomosc: " + wiadomosc);
                byte[] message = new ASCIIEncoding().GetBytes(wiadomosc);
                client.GetStream().Write(message, 0, message.Length);
            }
        }

        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] { 's' });
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] { 'c' });
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] { 'c' });
            Thread.Sleep(1000);
            Console.ReadKey();
        }
    }
}
