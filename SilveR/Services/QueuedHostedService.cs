using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilveR.Services
{
    public sealed class QueuedHostedService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private Task _backgroundTask;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue)
        {
            TaskQueue = taskQueue;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _backgroundTask = Task.Run(BackgroundProceessing, CancellationToken.None);

            return Task.CompletedTask;
        }

        private async Task BackgroundProceessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(_shutdown.Token);

                await workItem(_shutdown.Token);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _shutdown.Cancel();

            return Task.WhenAny(_backgroundTask,
                Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public void Dispose()
        {
            _shutdown.Dispose();
        }
    }
}