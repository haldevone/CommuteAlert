namespace CommuteAlert.Web.Services;

public class AlertScheduler : BackgroundService
{
    private static readonly TimeZoneInfo Stockholm =
    TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<AlertScheduler> _logger;

    public AlertScheduler(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<AlertScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    //Poolar efter nya alerts och skickar oskickade alerts
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var seconds = _config.GetValue("Polling:IntervalSeconds", 61);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));

        do
        {
            try
            {
                var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Stockholm);
                using var scope = _scopeFactory.CreateScope();

                //Hämta nya alerts och skicka oskickade alerts
                var created = await scope.ServiceProvider.GetRequiredService<AlertService>()
                    .CheckAndGetAlerts(nowLocal, stoppingToken);
                await scope.ServiceProvider.GetRequiredService<AlertSender>()
                    .SendUnsentAlerts(nowLocal, stoppingToken);

                if (created.Count > 0)
                    _logger.LogInformation("{Count} new alerts", created.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Alert round failed");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }


}
