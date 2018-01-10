using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace prezentacja_zad15
{
    class ClientAndMessage
    {
        public string message;
        public TcpClient client;
        public ClientAndMessage(TcpClient client, string message)
        {
            this.message = message;
            this.client = client;
        }
        public void show()
        {
            Console.WriteLine("Klient " /*+ client.LocalEndPoint*/ +" nadal wiadomosc: " + message);
        }
    }
    class ServerLog
    {
        public List<ClientAndMessage> log_list;
        public ServerLog()
        {
            log_list = new List<ClientAndMessage>();
        }
        public void showLogs()
        {
            foreach (ClientAndMessage cam in log_list)
            {
                cam.show();
            }
            Console.WriteLine();
        }
    }
    class Server
    {
        private static Object thisLock = new Object();
        ServerLog serverLog;
        TcpListener server;
        int port;
        IPAddress address;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task serverTask;

        public Server()
        {
            address = IPAddress.Any;
            port = 2048;
            serverLog = new ServerLog();
        }

        public async Task RunAsync(CancellationToken ct)
        {

            server = new TcpListener(address, port);

            try
            {
                server.Start();

            }
            catch (SocketException ex)
            {
                throw (ex);
            }
            while (true && !ct.IsCancellationRequested)
            {

                TcpClient client = await server.AcceptTcpClientAsync();
                byte[] buffer = new byte[1024];
                using (ct.Register(() => client.GetStream().Close()))
                {
                    client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct).ContinueWith(
                        async (t) =>
                        {
                            int i = t.Result;
                            while (true)
                            {
                                client.GetStream().WriteAsync(buffer, 0, i, ct);
                                try
                                {
                                    i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct);
                                    
                                    lock (thisLock)
                                    {
                                        string mess = Encoding.UTF8.GetString(buffer, 0, i);
                                        serverLog.log_list.Add(new ClientAndMessage(client, mess));
                                    }
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        });
                }
            }

        }

        public void Run()
        {
            Console.WriteLine("Server - start");
            serverTask = RunAsync(cancellationTokenSource.Token);
        }
        public void Stop()
        {
            //Communicates a request for cancellation.
            cancellationTokenSource.Cancel();
            //Closes the listener.
            server.Stop();
            Console.WriteLine("Server - stop");
        }
        public ServerLog getServerLog()
        {
            return serverLog;
        }

    }
    class Client
    {
        TcpClient client;

        public void Connect()
        {
            client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
            Console.WriteLine("Client - connected");

        }
        //public async Task<bool> Ping()
        //{
        //    Console.WriteLine("Client - pinguje");
        //    bool pinging = false;
        //    Ping isPing = new Ping();
        //    PingReply reply = isPing.Send(IPAddress.Parse("127.0.0.1"));
        //    Thread.Sleep(1000);
        //    pinging = reply.Status == IPStatus.Success;
        //    return pinging;
        //}

       
        public async Task<string> Ping()
        {
            Console.WriteLine("Client - ping");
            Thread.Sleep(1000);
            byte[] buffer = new ASCIIEncoding().GetBytes("ping");
            client.GetStream().WriteAsync(buffer, 0, buffer.Length);
            buffer = new byte[1024];
            var t = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, t);
        }
        //public async Task<List<bool>> keepPinging(CancellationToken token)
        //{
        //    List<bool> mes = new List<bool>();
        //    bool done = false;
        //    while (!done)
        //    {
        //        if (token.IsCancellationRequested)
        //            done = true;
        //        mes.Add(await Ping());
        //    }
        //    return mes;
        //}
        public async Task<IEnumerable<string>> keepPinging(CancellationToken token)
        {
            List<string> messages = new List<string>();
            bool done = false;
            while (!done)
            {
                if (token.IsCancellationRequested)
                    done = true;
                messages.Add(await Ping());
            }
            return messages;
        }
     
        static void Main(string[] args)
        {
            Server s = new Server();
            s.Run();
            Client c1 = new Client();
            Client c2 = new Client();
            Client c3 = new Client();
            c1.Connect();
            c2.Connect();
            c3.Connect();

            CancellationTokenSource ct1 = new CancellationTokenSource();
            CancellationTokenSource ct2 = new CancellationTokenSource();
            CancellationTokenSource ct3 = new CancellationTokenSource();

            var t1 = c1.keepPinging(ct1.Token);
            var t2 = c2.keepPinging(ct2.Token);
            var t3 = c3.keepPinging(ct3.Token);

            var tasks = new List<Task<IEnumerable<string>>>();

            tasks.Add(t1);
            tasks.Add(t2);
            tasks.Add(t3);

            ct1.CancelAfter(2000);
            ct2.CancelAfter(3000);
            ct3.CancelAfter(4000);


            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("----------Logi Servera----------");
            ServerLog serverlog = s.getServerLog();
            serverlog.showLogs();
            s.Stop();
            Console.WriteLine("koniec main");
        }
    }

}

