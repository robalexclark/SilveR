using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SilveR.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public sealed class BackgroundTaskQueue : IBackgroundTaskQueue, IDisposable
    {
        private readonly Channel<Func<CancellationToken, Task>> _workItems = Channel.CreateUnbounded<Func<CancellationToken, Task>>();

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);

            if (!_workItems.Writer.TryWrite(workItem))
            {
                throw new InvalidOperationException("The background task queue is closed.");
            }
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _workItems.Reader.ReadAsync(cancellationToken);
        }

        public void Dispose()
        {
            _workItems.Writer.TryComplete();
        }
    }
}
