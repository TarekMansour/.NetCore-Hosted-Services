using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Services
{
    public class ConsumeJobHostedService : IHostedService, IDisposable
    {
        public IServiceProvider Services { get; }
        private TelemetryClient _telemetryClient;
        private Timer _timer;

        public ConsumeJobHostedService(IServiceProvider services, TelemetryClient telemetryClient)
        {
            Services = services;
            _telemetryClient = telemetryClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telemetryClient.TrackEvent("Job Hosted Service is running.");
            try
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace($"{ex} catched while executing the BackgroundService task!");
                throw;
            }
        }

        private void DoWork(object state)
        {
            _telemetryClient.TrackEvent("Job Hosted Service is working.");

            using (_telemetryClient.StartOperation<RequestTelemetry>("operation"))
            {
                using (var scope = Services.CreateScope())
                {
                    var jobService =
                        scope.ServiceProvider
                            .GetRequiredService<IJobService>();

                    jobService.DoWork();
                }
                _telemetryClient.TrackEvent("Task completed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telemetryClient.TrackEvent("Job Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
