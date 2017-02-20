using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternProcessorLib
{
    class LibSettings
    {
        //this key is needed: HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\NC\ExternalProcessor
        private string _keyRootName = @"Software\NC\ExternalProcessor";
        private readonly RegistryKey _rootKey;

        public LibSettings()
        {
            _rootKey = Registry.LocalMachine.OpenSubKey(_keyRootName);
        }

        internal  string GetRubyExePath()
        {
            return GetValue<string>("RubyExePath", null);
        }

        #region Registry
        private T GetValue<T>(string key, T defaultValue)
        {
            if (typeof(T) == typeof(double))
            {
                return (T)(object)GetDoubleValue(key, (double)(object)defaultValue);
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)GetBooleanValue(key, (bool)(object)defaultValue);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)GetIntValue(key, (int)(object)defaultValue);
            }

            if (_rootKey == null) { return defaultValue; }
            return (T)_rootKey.GetValue(key, defaultValue);
        }

        private object GetIntValue(string key, int defaultValue)
        {
            var valueString = GetValue<string>(key, null);
            if (valueString == null) { return defaultValue; }

            int intValue;
            if (int.TryParse(valueString, out intValue))
            {
                return intValue;
            }
            else
            {
                return defaultValue;
            }
        }

        private bool GetBooleanValue(string key, bool defaultValue)
        {
            var valueString = GetValue<string>(key, null);
            if (valueString == null) { return defaultValue; }

            bool boolValue;
            if (Boolean.TryParse(valueString, out boolValue))
            {
                return boolValue;
            }
            else
            {
                return defaultValue;
            }
        }

        private double GetDoubleValue(string key, double defaultValue)
        {
            var valueString = GetValue<string>(key, null);
            if (valueString == null) { return defaultValue; }

            double doubleValue;
            if (Double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue))
            {
                return doubleValue;
            }
            else
            {
                return defaultValue;
            }
        }
        #endregion

    }
}
