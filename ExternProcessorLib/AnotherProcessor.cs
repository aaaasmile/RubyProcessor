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
        private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(AnotherProcessor));

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

        public void StartProcess(Dictionary<string, object> env, string template)
        {

            ProcessStarter processStarter = new ProcessStarter();
            processStarter.OutputWrittenEvent += (x) => _result.AppendLine(x);


            string fileName = GenerateAppScript(env, template);

            //processStarter.ExecuteCmd(@"C:\local\share\ruby_2_3_3\bin\ruby.exe", @"c/Users/igsa/AppData/Local/Temp/tmplate_1f822f0c - 4a48 - 469e-a436 - d79b460ceb77.rb1");
            processStarter.ExecuteCmd(@"C:\local\share\ruby_2_3_3\bin\ruby.exe", fileName);


            _result.Append("Done!");
            FireTeminatedEvent();
        }

        private string GenerateAppScript(Dictionary<string, object> env, string template)
        {
            string result = Path.GetTempPath();
            result = Path.Combine(result, string.Format("tmplate_{0}.rb", Guid.NewGuid()));

            StreamWriter file = new StreamWriter(result);

            StringBuilder flyScript = new StringBuilder();
            flyScript.AppendLine("require 'rubygems'");
            flyScript.AppendLine("require 'erubis'");

            //flyScript.AppendLine("");
            //flyScript.AppendLine("name = \"Igor\"");
            PrepareVariables(env, flyScript);
            flyScript.AppendLine(string.Format("input = \"{0}\"", template));
            flyScript.AppendLine("eruby_object= Erubis::Eruby.new(input)");
            flyScript.AppendLine("puts eruby_object.result(binding)");

            file.WriteLine(flyScript.ToString());
            file.Close();
            //flyScript.AppendLine("puts \"Hello World\"");
            //flyScript.AppendLine("puts \"Is not Fun?\"");
            _log.DebugFormat("Created script file {0}", result);

            return result;
        }

        private void PrepareVariables(Dictionary<string, object> env, StringBuilder flyScript)
        {
            foreach (var item in env)
            {
                string key = item.Key;
                var obj = item.Value;
                if (obj is List<string>)
                {
                    BuildStringArray(obj as List<string>, flyScript, key);
                }
                else if (obj is List<Dictionary<string, string>>)
                {
                    BuildRecordArray(obj as List<Dictionary<string, string>>, flyScript, key);
                }
                else if (obj is Dictionary<string, string>)
                {
                    BuildSingleRecord(obj as Dictionary<string, string>, flyScript, key);
                }
                else if (obj is string)
                {
                    BuildStringAssignment(obj as string, flyScript, key);
                }
            }
        }

        private void BuildStringAssignment(string v, StringBuilder flyScript, string key)
        {
            flyScript.AppendFormat("{0} = \"{1}\"\n", key, v);
        }

        private void BuildStringArray(List<string> list, StringBuilder flyScript, string key)
        {
            flyScript.AppendFormat("{0} = [{1}]", key, ConcatenateStrings(list));
        }

        private void BuildSingleRecord(Dictionary<string, string> record, StringBuilder flyScript, string key)
        {
            flyScript.AppendFormat("{0} = { ", key);
            BuildHashPart(record, flyScript);
            flyScript.AppendFormat("}\n");
        }

        private void BuildRecordArray(List<Dictionary<string, string>> list, StringBuilder flyScript, string key)
        {
            flyScript.AppendFormat("{0} = [ ", key);
            int count = 0;
            foreach (Dictionary<string, string> item in list)
            {
                if (count == 0)
                    BuildHashPart(item, flyScript);
                else
                {
                    flyScript.AppendFormat(", ");
                    BuildHashPart(item, flyScript);
                }

                count += 1;
            }
            flyScript.AppendFormat(" ]\n");
        }

        private static void BuildHashPart(Dictionary<string, string> record, StringBuilder flyScript)
        {
            int count = 0;
            foreach (var item in record)
            {
                if (count == 0)
                    flyScript.AppendFormat("{0} => \"{1}\"", item.Key, item.Value);
                else
                    flyScript.AppendFormat(", {0} => \"{1}\"", item.Key, item.Value);

                count += 1;
            }
        }

        private string ConcatenateStrings(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var item in list)
            {
                if (count > 0)
                    sb.AppendFormat(", \"{0}\"", item);
                else
                    sb.AppendFormat("\"{0}\"", item);

                count += 1;
            }
            return sb.ToString();
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
