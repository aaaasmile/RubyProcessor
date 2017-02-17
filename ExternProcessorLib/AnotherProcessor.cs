using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternProcessorLib
{
    public class AnotherProcessor
    {
        private StringBuilder _result = new StringBuilder();

        public delegate void ProcessTerminatedHandler(string Result);
        public event ProcessTerminatedHandler TeminatedEvent;

        public void StartProcess(Dictionary<string, string> Env, string Template)
        {

            ProcessStarter processStarter = new ProcessStarter();
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
    }
}
