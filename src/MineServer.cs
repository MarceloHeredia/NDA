using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NDA
{
    class MineServer
    {
        public static Task StartServer()
        {
            Process cmdProcess = new Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.CreateNoWindow = false;
            cmdProcess.StartInfo.RedirectStandardError = true;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.RedirectStandardInput = true;
            cmdProcess.StartInfo.UseShellExecute = false;


            cmdProcess.Start();

            cmdProcess.BeginOutputReadLine();

            StreamWriter cmdWriter = cmdProcess.StandardInput;

            cmdWriter.WriteLineAsync("cd D:\\Games\\Server1.14");
            cmdWriter.WriteLineAsync("java -jar paper-218.jar");

            return Task.CompletedTask;

        }
        /// <summary>Calls JPS and </summary>
        /// <returns></returns>
        private static Boolean IsTheServerRunning()
        {

        }
    }
}
