using System;
using System.Diagnostics;

namespace Id3Tag
{
    /// <summary>
    /// Provides a log mechanism for the ID3 operations. Enable the System.Diagnostics.Trace logger
    /// if you want to use this logging feature.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Enable / Disable the logging.
        /// </summary>
        public static bool EnableLogging { get; set; }

        internal static void LogInfo(string message)
        {
            Log("Info", message);
        }

        internal static void LogWarning(string message)
        {
            Log("Warning", message);
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
            if (EnableLogging)
            {
                string log = String.Format("{0} - {1} : {2}", DateTime.Now.ToLongTimeString(), level, message);
                Trace.WriteLine(log);
            }
        }
    }
}