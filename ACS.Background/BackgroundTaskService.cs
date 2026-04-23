

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACS.Background
{
    /// <summary>
    /// Background service that processes tasks from a background task queue.
    /// </summary>
    /// <param name="backgroundTaskQueue"></param>
    /// <param name="logger"></param>
    public class BackgroundTaskService(IBackgroundTaskQueue backgroundTaskQueue, ILogger<BackgroundTaskService> logger) : BackgroundService
    {
        /// <summary>
        /// Execute Async
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await backgroundTaskQueue.DequeueAsync(stoppingToken);
                if (workItem != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await workItem(stoppingToken);
                        }
                        catch (TaskCanceledException) { }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error executing task.");
                        }
                    }, stoppingToken);
                }
            }
        }
    }
}
