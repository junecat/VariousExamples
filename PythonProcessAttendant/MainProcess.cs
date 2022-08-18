using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.Management;

namespace pp
{
    internal class MainProcess
    {
        public void MainCircuit()
        {
            try
            {

                const string anyCommand = "python tbot.py";

                while (true)
                {
                    bool allOk = false;

                    Process[] processCollection = Process.GetProcesses();

                    foreach (Process p in processCollection)
                    {
                        if (p.ProcessName.ToLower() == "python")
                        {
                            // получаем аргумент
                            string arg = p.GetCommandLine();
                            arg = arg.Replace("  ", " ");
                            if (arg == "python tbot.py")
                                allOk = true;
                        }
                    }

                    if (!allOk)
                    {
                        Log.Information("Process need to start!");

                        var proc1 = new ProcessStartInfo();
                        proc1.UseShellExecute = true;

                        //proc1.WorkingDirectory = @"C:\Windows\System32";
                        proc1.WorkingDirectory = @"C:\MyProjects\Experiments\TeleBots";

                        proc1.FileName = @"C:\Windows\System32\cmd.exe";
                        //proc1.Verb = "runas";
                        proc1.Arguments = "/c " + anyCommand;
                        proc1.WindowStyle = ProcessWindowStyle.Minimized;
                        Process.Start(proc1);
                    }

                    Thread.Sleep(30000);

                }

            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
        }



    }
}
