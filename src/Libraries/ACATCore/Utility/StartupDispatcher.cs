using System;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Keeps plugin construction and UI initialization on the startup thread.
    /// </summary>
    internal sealed class StartupDispatcher
    {
        private readonly SynchronizationContext _context = SynchronizationContext.Current;
        private readonly int _threadId = Thread.CurrentThread.ManagedThreadId;

        public Task<T> InvokeAsync<T>(Func<T> action)
        {
            return InvokeAsync(() => Task.FromResult(action()));
        }

        public Task<T> InvokeAsync<T>(Func<Task<T>> action)
        {
            if (Thread.CurrentThread.ManagedThreadId == _threadId)
            {
                return action();
            }

            if (_context == null)
            {
                throw new InvalidOperationException("Async startup requires a SynchronizationContext on the owning thread.");
            }

            var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            _context.Post(async _ =>
            {
                try
                {
                    completion.TrySetResult(await action().ConfigureAwait(false));
                }
                catch (OperationCanceledException)
                {
                    completion.TrySetCanceled();
                }
                catch (Exception exception)
                {
                    completion.TrySetException(exception);
                }
            }, null);
            return completion.Task;
        }
    }
}
