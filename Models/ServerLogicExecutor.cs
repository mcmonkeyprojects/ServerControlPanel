using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ServerControlPanel.Models
{
    public class ServerLogicExecutor
    {
        public static bool ShouldBeRunning = false;

        public static bool StatusCheck()
        {
            Console.WriteLine("Status check initiated");
            ProcessStartInfo procStart = new("bash", "./config/status_check.sh")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Process statusProc = Process.Start(procStart);
            StreamReader reader = statusProc.StandardOutput;
            statusProc.WaitForExit();
            string result = reader.ReadToEnd();
            reader.Close();
            bool resultBool = result.ToLowerInvariant().Trim() == "true";
            Console.WriteLine($"Status check result: {resultBool}");
            return resultBool;
        }

        public static void Start()
        {
            Console.WriteLine("Start task initiated");
            Process startProc = Process.Start("bash", "./config/start.sh");
            startProc.WaitForExit();
            ShouldBeRunning = true;
        }

        public static void Restart()
        {
            Console.WriteLine("Restart task initiated");
            Process restartProc = Process.Start("bash", "./config/restart.sh");
            restartProc.WaitForExit();
            ShouldBeRunning = true;
        }

        public static void Stop()
        {
            Console.WriteLine("Stop task initiated");
            ShouldBeRunning = false;
            Process stopProc = Process.Start("bash", "./config/stop.sh");
            stopProc.WaitForExit();
        }
    }
}
