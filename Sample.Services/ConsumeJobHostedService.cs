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
        private Timer _timer;

        public ConsumeJobHostedService(IServiceProvider services)
        {
            Services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DoWork(object state)
        { 
            using (var scope = Services.CreateScope())
            {
                var jobService =
                    scope.ServiceProvider
                    .GetRequiredService<IJobService>();
                jobService.DoWork();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
