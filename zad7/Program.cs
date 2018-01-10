using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zad7
{
    class Program
    {
        

        static void Main(string[] args)
        {
            FileStream fs;
          
            try
            {
                fs = new FileStream("plik.txt", FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("nie znaleziono");
                return;
            }
            byte[] buffer = new byte[fs.Length];

            IAsyncResult asyncResult = fs.BeginRead(buffer, 0, buffer.Length, null, new object[] { fs, buffer});

            fs.EndRead(asyncResult);

            string tekstZPliku = Encoding.ASCII.GetString(buffer);

            Console.WriteLine(tekstZPliku);
            Console.ReadKey();
        }
    }
}
