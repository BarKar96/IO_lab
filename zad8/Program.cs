using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zad8
{
    class Program
    {
        static int Silnia_rek(int n)
        {
            if (n > 1)
            {
                return Silnia_rek(n - 1) * n;
            }
            else
            {
                return 1;
            }
        }
       

        static int Silnia_ite(int n)
        {
            int liczba, wynik = 1;
            if (n == 0)
            {
                return 1;
            }
            else
            {
                while (n > 0)
                {
                    wynik *= n;
                    n--;
                }
                return wynik;
            }
        }

        static int Fibonacci_ite(int number)
        {
            int pop = 0;
            int next = 1;
            int temp;
            for (int i = 1; i < number; i++)
            {
                temp = next;
                next = pop + next;
                pop = temp;
            }
            return next;
        }
        static int Fibonacci_rek(int x)
        {

            if (x == 0)
                return 0;
            else if (x == 1)
                return 1;
            else
            {
                return Fibonacci_rek(x - 2) + Fibonacci_rek(x - 1);
            }
        }
        delegate int DelegateType(int number);
        static DelegateType silnia_rek_del;
        static DelegateType silnia_ite_del;
        static DelegateType fibonacci_ite_del;
        static DelegateType fibonacci_rek_del;

        static void Main(string[] args)
        {
            silnia_rek_del = new DelegateType(Silnia_rek);
            silnia_ite_del = new DelegateType(Silnia_ite);
            fibonacci_ite_del = new DelegateType(Fibonacci_ite);
            fibonacci_rek_del = new DelegateType(Fibonacci_rek);

            IAsyncResult iasyncResult1 = silnia_rek_del.BeginInvoke(5, null, null);
            IAsyncResult iasyncResult2 = silnia_ite_del.BeginInvoke(5, null, null);
            IAsyncResult iasyncResult3 = fibonacci_ite_del.BeginInvoke(5, null, null);
            IAsyncResult iasyncResult4 = fibonacci_rek_del.BeginInvoke(5, null, null);

            int result1 = silnia_rek_del.EndInvoke(iasyncResult1);
            int result2 = silnia_ite_del.EndInvoke(iasyncResult2);
            int result3 = fibonacci_ite_del.EndInvoke(iasyncResult3);
            int result4 = fibonacci_rek_del.EndInvoke(iasyncResult4);

            Console.WriteLine(result1);
            Console.WriteLine(result2);
            Console.WriteLine(result3);
            Console.WriteLine(result4);
            Console.ReadKey();
        }
    }
}
