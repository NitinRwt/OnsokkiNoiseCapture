using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
namespace OnsokkiNoiseCaptureDemo
{
    class OnosokkiControl
    {

        private const string PipeName = "OnosokkiPipe";

        public static void updateOnosokkiStart()
        {
            try
            {
                SendCommand("start");
            }
            catch (Exception exp)
            {
                throw exp;
            }

        }

        public static void updateOnosokkiStop()
        {
            try
            {
                SendCommand("stop");
            }
            catch (Exception exp)
            {
                throw exp;
            }

        }

        private static void SendCommand(string command)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    pipeClient.Connect(1000); // Timeout after 1 second
                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(command);
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
                // Optionally log or show an error: pipe not available
            }
        }

        public static void clearCommand()
        {
            string processName = "ConsoleApp1";
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(); // optional
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}

