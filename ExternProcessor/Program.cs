using ExternProcessorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> env = new Dictionary<string, string>();
            AnotherProcessor proc = new AnotherProcessor();
            proc.TeminatedEvent += (x) => { Console.WriteLine(x); };

            proc.InitLogger();
            proc.StartProcess(env, "");

            Console.WriteLine("Press any Key");
            Console.ReadKey();
        }
    }
}
