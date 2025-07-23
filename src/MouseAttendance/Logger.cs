using System;
using System.IO;

namespace MouseAttendance
{
    public static class Logger
    {
        private static readonly object _sync = new object();

        public static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logMessage = $"{timestamp} {message}";
            Console.WriteLine(logMessage);
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var logDir = Path.Combine(baseDir, "logs");
                Directory.CreateDirectory(logDir);
                var logFile = Path.Combine(logDir, DateTime.Now.ToString("yyMMdd") + ".log");
                lock (_sync)
                {
                    File.AppendAllText(logFile, logMessage + Environment.NewLine);
                }
            }
            catch
            {
                // ignore logging failures
            }
        }
    }
}
