using ACS.Models;
using ACS.Service.V1.Interfaces;
using System.Collections.Concurrent;

namespace ACS.Background
{
    /// <summary>
    /// Provides a thread-safe queue for managing and processing background work items in the form of asynchronous
    /// tasks.
    /// </summary>
    /// <remarks>Use this class to enqueue background operations that should be executed sequentially or by
    /// background services. Each work item is represented as a delegate that accepts a CancellationToken and returns a
    /// Task, allowing for cooperative cancellation and asynchronous execution. This class is typically used in
    /// scenarios where background processing is required, such as in web applications for handling long-running or
    /// deferred tasks.</remarks>
    public class BackgroundTaskQueue(ILicenseManager license) : ACS.Service.Service(license), IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
        private readonly SemaphoreSlim _signal = new(0);


        /// <summary>
        /// Queues the specified work item to run in the background on a thread pool thread.
        /// </summary>
        /// <remarks>The queued work item may run concurrently with other work items. The provided
        /// CancellationToken should be monitored by the work item to support cooperative cancellation. Exceptions
        /// thrown by the work item may not be observed by the caller.</remarks>
        /// <param name="workItem">A function that represents the background work to execute. The function receives a CancellationToken that is
        /// signaled if the work item should be canceled. Cannot be null.</param>
        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            _workItems.Enqueue(workItem);
            _signal.Release();
        }


        /// <summary>
        /// Asynchronously retrieves the next available work item from the queue as a delegate to be executed.
        /// </summary>
        /// <remarks>The returned delegate encapsulates the logic for the work item and should be invoked
        /// by the caller. Multiple calls to this method may wait if the queue is empty. This method is thread-safe and
        /// can be called concurrently from multiple threads.</remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the dequeue operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a delegate that, when invoked
        /// with a cancellation token, executes the dequeued work item. If the queue is empty, the task will complete
        /// when a work item becomes available or the operation is canceled.</returns>
        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            if (_workItems.TryDequeue(out var workItem) && workItem is not null)
            {
                return workItem;
            }
            throw new InvalidOperationException("No work item was available to dequeue.");
        }
    }
}

