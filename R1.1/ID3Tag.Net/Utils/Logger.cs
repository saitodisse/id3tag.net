using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Id3Tag
{
    internal static class Logger
    {
        internal static void LogInfo(string message)
        {
            Log("Info", message);
        }

        internal static void LogWarning(string message)
        {
            Log("Warning",message);
        }

        internal static void LogError(string message)
        {
            Log("Error", message);
        }

        internal static void LogError(Exception ex)
        {
            Log("Error", ex.ToString());
        }

        private static void Log(string level, string message)
        {
            var log = String.Format("{0} - {1} : {2}", DateTime.Now.ToLongTimeString(),level, message);
            Trace.WriteLine(log);
        }
    }
}
