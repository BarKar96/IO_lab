using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zad1
{
    class Program
    {
        static void ThreadProc(Object stateInfo)
        {
            //Pobierając dane nie zawsze musimy jawnie deklarować ich typ
            var time = ((object[])stateInfo)[0];
            Thread.Sleep((int)time);
            //mimo wszystko dane są przekazane do wątku poprawnie
            Console.WriteLine("Watek - czekalem: "+time);
        }


        static void Main(string[] args)
        {
            //Przykładowy sposób przekazania danych do wątku
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] {500});
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] {300});
            Thread.Sleep(1000);
            Console.WriteLine("koniec Main");
            Console.ReadKey();
        }
        
    }
}
