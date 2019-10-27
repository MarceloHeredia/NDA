using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NDA
{
    class MineServer
    {
        public static async Task<Task> StartServer()
        {
            if (IsTheServerRunning())
            {
                return Task.FromException(new Exception("O servidor está em execução."));
            }
            Process cmdProcess = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                }
            };
            cmdProcess.Start();

            cmdProcess.BeginOutputReadLine();

            StreamWriter cmdWriter = cmdProcess.StandardInput;

            await cmdWriter.WriteLineAsync("cd D:\\Games\\Server1.14");
            await cmdWriter.WriteLineAsync("java -jar paper-218.jar");

            cmdProcess.Close();

            return Task.CompletedTask;

        }
        /// <summary>Calls JPS and </summary>
        /// <returns></returns>
        private static Boolean IsTheServerRunning()
        {
            var check = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = @"/c jps -l",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = false,
                },
            };
            check.OutputDataReceived += (a, b) => Console.WriteLine(b.Data);
            check.ErrorDataReceived += (a, b) => Console.WriteLine(b.Data);
            check.Start();

            var output = check.StandardOutput.ReadToEnd();

            var arrOutput = output.Split("\r\n");

            var server = arrOutput.SingleOrDefault(to => to.Contains("paper-218"));

            if (!String.IsNullOrEmpty(server))
            {
                return true;
            }
            check.WaitForExit();
            check.CloseMainWindow();
            check.Close();

            return false;
        }

        public static async Task<Task> Stop()
        {
            Int32 pid = GetServerPid();
            if (pid == -1)
            {
                return Task.FromException(new Exception("O servidor não está em execução."));
            }
            var check = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                },
            };

            check.Start();
            check.BeginOutputReadLine();

            StreamWriter cmdWriter = check.StandardInput;

            await cmdWriter.WriteLineAsync(String.Concat("taskkill /F /PID ", pid)) ;

            check.CloseMainWindow();
            check.Close();
            return Task.CompletedTask;
        }

        private static Int32 GetServerPid()
        {
            Int32 pid = -1;

            var check = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = @"/c jps -l",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = false,
                },
            };
            check.OutputDataReceived += (a, b) => Console.WriteLine(b.Data);
            check.ErrorDataReceived += (a, b) => Console.WriteLine(b.Data);
            check.Start();

            var output = check.StandardOutput.ReadToEnd();

            var arrOutput = output.Split("\r\n");

            var server = arrOutput.SingleOrDefault(to => to.Contains("paper-218"));

            if (!String.IsNullOrEmpty(server))
            {
                try
                {
                    pid = Convert.ToInt32(server.Split(" ").First());
                }
                catch (Exception)
                {
                    pid = -1;
                }
            }

            check.CloseMainWindow();
            check.Close();
            return pid;
        }
    }
}
