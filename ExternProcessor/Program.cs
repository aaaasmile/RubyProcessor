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
            Dictionary<string, object> env = new Dictionary<string, object>();
            CreateDemoEnv(env);

            AnotherProcessor proc = new AnotherProcessor();
            proc.TeminatedEvent += (x) => { Console.WriteLine(x); };

            proc.InitLogger();
            proc.StartProcess(env, CreateDemoTemplate());

            Console.WriteLine("Press any Key");
            Console.ReadKey();
        }

        private static void CreateDemoEnv(Dictionary<string, object> env)
        {
            var root = new Dictionary<string, string>();
            root["UserName"] = "IgSa";
            root["Inv. No."] = "INV-2344";
            env.Add("root", root);

            var set = new List<Dictionary<string, string>>();

            var record = new Dictionary<string, string>();
            record["No."] = "101000";
            record["Description"] = "The simple item";
            record["Price"] = "1000,00";
            record["Date"] = "01/02/2017";
            set.Add(record);

            var record2 = new Dictionary<string, string>();
            record2["No."] = "101001";
            record2["Description"] = "The second ";
            record2["Price"] = "2200,00";
            record2["Date"] = "02/02/2017";
            set.Add(record2);

            env.Add("dataset1", set);
        }

        private static string CreateDemoTemplate()
        {
            StringBuilder str = new StringBuilder();

            str.Append("This is the Template by <%= myname %>");

            return str.ToString();
        }
    }
}
