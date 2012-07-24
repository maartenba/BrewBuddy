using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrewBuddy.Worker.Logic;

namespace BrewBuddy.Worker.ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register ConsoleTraceListener so we can see what our worker is doing...
            Trace.Listeners.Add(new ConsoleTraceListener());

            // Start a new Thread for every task
            List<Task> threads = new List<Task>();
            threads.Add(new Task(() => new TemperatureTap().Run()));

            // Start the process
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Starting BrewBuddy worker logic...");
            foreach (var thread in threads)
            {
                thread.Start();
            }
            Console.WriteLine("Started BrewBuddy worker logic.");
            Console.WriteLine("Press 'q' at any time to end the process.");
            Console.WriteLine();
            Console.ResetColor();

            // Wait...
            while (Console.ReadKey(true).KeyChar != 'q')
            {
                Thread.Sleep(50);
            }
        }
    }
}
