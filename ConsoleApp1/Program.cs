using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static Stopwatch Sw = new Stopwatch();
        static void Main(string[] args)
        {
            Sw.Start();
            int i = 0;
            while (Sw.ElapsedMilliseconds < 1000)
            {
                i++;
            }
            Console.WriteLine("amount counted: " + i);
            Console.WriteLine("every tick lasted: " + 1.0/i * 1000000.0 + " µs");

            while (true) { }
        }
    }
}
