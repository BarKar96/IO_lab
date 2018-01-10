using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zad6
{
    class Program
    {
        public static void Handler(IAsyncResult asyncResult)
        {
            FileStream fileStream= (FileStream)((object[])asyncResult.AsyncState)[0];
            byte[] buffer = (byte[])((object[])asyncResult.AsyncState)[1];
            AutoResetEvent a = (AutoResetEvent)((object[])asyncResult.AsyncState)[2];
            string s = Encoding.ASCII.GetString(buffer);
            Console.WriteLine(s);
            a.Set();
            fileStream.EndRead(asyncResult);
   
        }


        static void Main(string[] args)
        {
            FileStream fs;
            WaitHandle waitHandle = new AutoResetEvent(false);
            try
            {
               fs = new FileStream("plik1.txt", FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("nie znaleziono");
                return;
            }
            byte[] buffer = new byte[fs.Length];

            fs.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(Handler), new object[] { fs, buffer, waitHandle });
            waitHandle.WaitOne();
            Console.ReadKey();
        }
    }
}
