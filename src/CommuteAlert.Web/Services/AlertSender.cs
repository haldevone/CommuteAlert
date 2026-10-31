using CommuteAlert.Web.Data;
using CommuteAlert.Web.Notifications;
using CommuteAlert.Web.Security;
using Microsoft.EntityFrameworkCore;

namespace CommuteAlert.Web.Services;

public class AlertSender
{
    private readonly INotifier _notifier;
    private readonly WebhookProtector _protector;
    private readonly ILogger<AlertSender> _logger;
    private readonly ApplicationDbContext _db;

    public AlertSender(INotifier notifier, WebhookProtector protector, ILogger<AlertSender> logger, ApplicationDbContext db)
    {
        _notifier = notifier;
        _protector = protector;
        _logger = logger;
        _db = db;
    }

    //Hämta oskickade alerts från databasen inom 30 minuter,
    //försök skicka dem via notifier och markera dem som skickade om det lyckas.
    public async Task SendUnsentAlerts(DateTime nowLocal, CancellationToken ct = default)
    {
        var since = nowLocal - TimeSpan.FromMinutes(30);
        var unsent = await _db.Alerts
            .Where(a => a.SentAt == null && a.CreatedAt >= since)
            .ToListAsync(ct);


        foreach (var alert in unsent)
        {
            var webhook = await _db.Watches
                .Where(w => w.Id == alert.WatchId)
                .Join(_db.Webhooks, w => w.UserId, h => h.UserId, (w, h) => h)
                .FirstOrDefaultAsync(ct);

            if (webhook == null) continue;

            string url;
            try
            {
                url = _protector.Unprotect(webhook.EncryptedUrl);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not decrypt webhook for watch {WatchId}", alert.WatchId);
                continue;
            }

            if (await _notifier.SendAsync(url, alert.Message, ct))
            {
                alert.SentAt = nowLocal;
            }
            else
            {
                _logger.LogWarning("Could not deliver alert {AlertId}", alert.Id);
            }

        }

        await _db.SaveChangesAsync(ct);
    }


}
