using ACS.Notifications.WebService.Services.V1.Interfaces;

namespace ACS.Notifications.WebService.HostedServices
{
    /// <summary>
    /// Long-running background service that calls
    /// <c>IUserNotificationsService.RunDailyMaintenanceAsync</c> once per day at
    /// the configured UTC time. Behaviour is deliberately conservative:
    ///
    ///   * On startup we wait <c>Maintenance:StartupDelaySeconds</c> (default 60s)
    ///     so the rest of the host has finished initialising and we don't add
    ///     load on a cold start.
    ///   * We then sleep until the next occurrence of <c>Maintenance:DailyRunUtc</c>
    ///     (HH:mm:ss). Default 02:00:00.
    ///   * On each tick we call the service. Any exception (other than
    ///     OperationCanceledException from host shutdown) is caught and logged
    ///     so a single failed run never crashes the host.
    ///   * Retention windows come from <c>Maintenance:ReadRetentionDays</c>
    ///     (default 90) and <c>Maintenance:UnreadRetentionDays</c> (default 365).
    ///   * Setting <c>Maintenance:Enabled = false</c> turns the loop off entirely.
    /// </summary>
    public sealed class DailyMaintenanceHostedService(
        IUserNotificationsService userNotificationsService,
        IConfiguration configuration,
        ILogger<DailyMaintenanceHostedService> logger) : BackgroundService
    {
        private const string SectionName = "Maintenance";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = configuration.GetValue<bool?>($"{SectionName}:Enabled") ?? true;
            if (!enabled)
            {
                logger.LogInformation("Daily maintenance is disabled via config (Maintenance:Enabled=false)");
                return;
            }

            var startupDelaySeconds = Math.Max(0,
                configuration.GetValue<int?>($"{SectionName}:StartupDelaySeconds") ?? 60);
            var runAtUtcStr  = configuration.GetValue<string?>($"{SectionName}:DailyRunUtc") ?? "02:00:00";
            var readDays     = Math.Max(0,
                configuration.GetValue<int?>($"{SectionName}:ReadRetentionDays")   ?? 90);
            var unreadDays   = Math.Max(0,
                configuration.GetValue<int?>($"{SectionName}:UnreadRetentionDays") ?? 365);

            if (!TimeSpan.TryParse(runAtUtcStr, out var runAtUtc) ||
                runAtUtc < TimeSpan.Zero || runAtUtc >= TimeSpan.FromDays(1))
            {
                logger.LogWarning(
                    "Maintenance:DailyRunUtc='{value}' is not a valid HH:mm:ss in [00:00:00,24:00:00); defaulting to 02:00:00",
                    runAtUtcStr);
                runAtUtc = new TimeSpan(2, 0, 0);
            }

            logger.LogInformation(
                "Daily maintenance scheduled at {time} UTC (read retention {read}d, unread retention {unread}d, startup delay {delay}s)",
                runAtUtc, readDays, unreadDays, startupDelaySeconds);

            try
            {
                if (startupDelaySeconds > 0)
                    await Task.Delay(TimeSpan.FromSeconds(startupDelaySeconds), stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var delay = ComputeDelayUntilNext(DateTime.UtcNow, runAtUtc);
                    logger.LogInformation(
                        "Next daily maintenance run in {delay} (at {next:o})",
                        delay, DateTime.UtcNow + delay);

                    try
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    // Run the maintenance — the service itself catches and
                    // returns failures so we only ever see exceptions for
                    // truly unexpected cases (e.g. DI resolution issues).
                    try
                    {
                        var (success, detail) = await userNotificationsService
                            .RunDailyMaintenanceAsync(readDays, unreadDays, stoppingToken);

                        if (success)
                            logger.LogInformation("Daily maintenance run completed: {detail}", detail);
                        else
                            logger.LogWarning("Daily maintenance run reported failure: {detail}", detail);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Defensive: never let a single failure tear down the host.
                        logger.LogError(ex, "Unexpected exception during daily maintenance run; will try again tomorrow");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown.
            }

            logger.LogInformation("DailyMaintenanceHostedService stopping");
        }

        /// <summary>
        /// Returns the TimeSpan from <paramref name="nowUtc"/> to the next
        /// occurrence of <paramref name="runAtUtc"/>. If we're already past
        /// today's slot, schedule for tomorrow. Always returns at least 1 second
        /// so a freshly-started host that lands exactly on the schedule still
        /// completes its initial setup before firing.
        /// </summary>
        internal static TimeSpan ComputeDelayUntilNext(DateTime nowUtc, TimeSpan runAtUtc)
        {
            var todaySlot = nowUtc.Date + runAtUtc;
            var next = nowUtc < todaySlot ? todaySlot : todaySlot.AddDays(1);
            var delay = next - nowUtc;
            return delay < TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : delay;
        }
    }
}
