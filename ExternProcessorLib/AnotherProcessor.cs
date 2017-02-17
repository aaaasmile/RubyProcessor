using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExternProcessorLib
{
    public class AnotherProcessor
    {
        private StringBuilder _result = new StringBuilder();

        public delegate void ProcessTerminatedHandler(string Result);
        public event ProcessTerminatedHandler TeminatedEvent;

        public void InitLogger()
        {
            string filename = Log4NetConfigFileName();
            if (File.Exists(filename))
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(filename));
            }
        }

        public void StartProcess(Dictionary<string, string> Env, string Template)
        {

            ProcessStarter processStarter = new ProcessStarter();
            processStarter.OutputWrittenEvent += (x) => _result.AppendLine(x);

            processStarter.ExecuteCmd(@"C:\local\share\ruby_2_3_3\bin\ruby.exe", "-v");


            _result.Append("Done!");
            FireTeminatedEvent();
        }

        private void FireTeminatedEvent()
        {
            if (TeminatedEvent != null)
            {
                TeminatedEvent(_result.ToString());
            }
        }

        private static string Log4NetConfigFileName()
        {
            string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string name = "ExternProcessorLib";

            return Path.Combine(
                dir,
                name + ".Log4net.config");
        }
    }
}
