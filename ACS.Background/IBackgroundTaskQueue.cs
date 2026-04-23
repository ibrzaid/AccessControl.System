

namespace ACS.Background
{
    /// <summary>
    /// Defines a contract for a queue that manages background work items to be processed asynchronously.
    /// </summary>
    /// <remarks>Implementations of this interface are typically used to coordinate the scheduling and
    /// execution of background tasks in hosted services or background processing scenarios. The queue is intended for
    /// use in multi-threaded environments and should be thread-safe.</remarks>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Queues the specified work item to run in the background on a thread pool thread.
        /// </summary>
        /// <remarks>The queued work item may run concurrently with other work items. The provided
        /// CancellationToken should be monitored by the work item to support cooperative cancellation. Exceptions
        /// thrown by the work item may not be observed by the caller.</remarks>
        /// <param name="workItem">A function that represents the background work to execute. The function receives a CancellationToken that is
        /// signaled if the work item should be canceled. Cannot be null.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);


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
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
