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
        public static int array_length = 102;
        public static int array_fragment_length = 25;
        public static int[] array = new int[array_length];
        public static int final_result = 0;
        private static Object thisLock = new Object();
        


        static void ThreadProc(Object stateInfo)
        {
            var temp1 = ((object[])stateInfo)[1];
            var temp2 = ((object[])stateInfo)[2];
            int start = Convert.ToInt32(temp1);
            int stop = Convert.ToInt32(temp2);
            int result = 0;
            for (int i=start; i<stop; i++)
            {
                if (i < array_length)
                {
                    result += array[i];
                }
                
            }
            final_result += result;
            Console.WriteLine("Watek: w danym fragmencie suma: "+result);

            AutoResetEvent waitHandle = (AutoResetEvent)((object[])stateInfo)[3];
            waitHandle.Set();


        }
        public void uzupelnijTabliceLiczb()
        {
            Random rnd = new Random();
            for (int i = 0; i < array_length; i++)
            {
                array[i] = rnd.Next(0,100);
            }
        }
        static void Main(string[] args)
        {
           
           
            Program program = new Program();
            program.uzupelnijTabliceLiczb();

            //obliczanie ilosci potrzebnych watkow
            float cnt = array_length / array_fragment_length;
            int rounded = (int)Math.Ceiling(cnt);

         
            int counter = 1;
            int handles_counter = 0;

            //tworzenie oraz uzupelnienie tablicy "sygnalow"
            WaitHandle[] waitHandles = new WaitHandle[rounded+1];
            for (int i=0;i<rounded+1; i++)
            {
                waitHandles[i] = new AutoResetEvent(false);
            }

            //obliczanie sumy fragmentow tablic
            for (int i = 0; i < array_length; i++)
            {
                if (i == 0)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), new object[] { 500, i, array_fragment_length, waitHandles[handles_counter] });
                    handles_counter++;
                    counter++; 
                }
                else if (i % array_fragment_length == 0)
                {
                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), new object[] { 500, i, array_fragment_length * counter, waitHandles[handles_counter] });
                    handles_counter++;
                    counter++;
                    
                }
                
            }

            //czekamy na sygnal o zakonczeniu pracy od wszystkich watkow
            WaitHandle.WaitAll(waitHandles);

            //koncowy wynik
            Console.WriteLine();
            Console.WriteLine("Ogolna suma: " + final_result);
            Console.ReadKey();
        }

       
    }
}

