using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace prezentacja_zad12
{
    class Program
    {
        public class TResultDataStructure
        {
           public int a { get; set; }
           public int b { get; set; }
        };

        public static async Task<TResultDataStructure> OperationTask(byte[] buffer)
        {
            TResultDataStructure tres = new TResultDataStructure();
            Console.WriteLine("begin task");
            await Task.Run(() =>
            {
                Console.WriteLine("begin async");
                Thread.Sleep(100);
                tres.a = 3;
                tres.b = 4;
                Console.WriteLine("end async");
                
                
            });
            Console.WriteLine("end task");
            return tres;

        }

        static void Main(string[] args)
        {
            int test = 0;
            byte[] buffer = new byte[128];
            Console.WriteLine("begin main");
            Task task = OperationTask(buffer);
            Thread.Sleep(test);
            Console.WriteLine("progress main");
            task.Wait();
            Console.WriteLine("end main");
        }
    }
}
