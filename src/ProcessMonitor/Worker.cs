using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now}");
                var allProcesses = Process.GetProcesses();
                var process = allProcesses.FirstOrDefault(p => p.ProcessName.Equals("teamredminer"));

                if (process == null)
                {
                    Invoke(@"cd C:\teamredminer-v0.8.5-win\; .\start_eth.bat");
                }

                await Task.Delay(500000, stoppingToken);
            }
        }

        private void Invoke(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", command)
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                }
            };

            process.Start();
        }
    }
}
