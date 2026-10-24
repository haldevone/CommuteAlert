using CommuteAlert.Web.Data;
using CommuteAlert.Web.Models;
using CommuteAlert.Web.Rules;
using CommuteAlert.Web.Sl;
using CommuteAlert.Web.Sl.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CommuteAlert.Web.Services;

public class AlertService
{

    private readonly ApplicationDbContext _db;
    private readonly ISlApiService _sl;
    private readonly ILogger<AlertService> _logger;

    public AlertService(ApplicationDbContext db, ISlApiService sl, ILogger<AlertService>? logger = null)
    {
        _db = db;
        _sl = sl;
        _logger = logger;
    }

    //Hämtar SL-data och skapar nya alerts.
    public async  Task<List<Alert>> CheckAndGetAlerts(DateTime nowLocal, CancellationToken ct = default)
    {
        var created = new List<Alert>();

        // Hämtar alla aktiva watches (bevakningar) som är i "window" just nu. Vi vill inte hämta alla watches,
        // eftersom det kan vara väldigt många
        var watches = (await _db.Watches.Where(w => w.IsActive).ToListAsync(ct))
            .Where(w => AlertRules.InWindow(w, nowLocal))
            .ToList();

        if (watches.Count == 0) return created;

        List<SlDeviation> deviations;

        // Hämta avvikelser (deviations) från SL. Om det inte går, logga och fortsätt med tom lista.
        try
        {
            deviations = await _sl.GetDeviationsAsync(ct);
        }
        catch (Exception ex) when (IsSlError(ex, ct))
        {
            _logger?.LogWarning("Could not fetch deviations: {Error}", ex.GetType().Name);
            
            deviations = new List<SlDeviation>();
        }

        var departuresBySite = new Dictionary<int, List<SlDeparture>>();

        // Hämta departures för varje unik siteId som finns i watches, och spara i en dictionary.
        // Om det inte går att hämta departures för en site, logga och fortsätt med tom lista.
        foreach (var siteId in watches.Select(w => w.SiteId).Distinct())
        {
            try
            {
                departuresBySite[siteId] = await _sl.GetDeparturesAsync(siteId, ct);
            }
            catch (Exception ex) when (IsSlError(ex, ct))
            {
                _logger?.LogWarning("Could not fetch departures for site {SiteId}: {Error}",siteId, ex.GetType().Name);
            }
        }

        // Utvärdera varje watch mot departures och deviations, och skapa alerts om det behövs,
        // undvik dubbletter, spara i databasen och returnera listan med skapade alerts.
        foreach (var watch in watches)
        {
            var departures = departuresBySite.TryGetValue(watch.SiteId, out var found)
                ? found
                : new List<SlDeparture>();

            var suggestions = AlertRules.Evaluate(watch, departures, deviations, nowLocal);

            var sentKeys = (await _db.Alerts
                .Where(a => a.WatchId == watch.Id)
                .Select(a => a.Key)
                .ToListAsync(ct)).ToHashSet();

            foreach (var s in suggestions)
            {
                if (!sentKeys.Add(s.Key)) continue;

                var alert = new Alert
                {
                    WatchId = watch.Id,
                    Type = s.Type,
                    Key = s.Key,
                    Message = s.Message,
                    CreatedAt = nowLocal
                };
                _db.Alerts.Add(alert);
                created.Add(alert);
            }
        }

        await _db.SaveChangesAsync(ct);
        return created;
    }

    private static bool IsSlError(Exception ex, CancellationToken ct) =>
    !ct.IsCancellationRequested &&
    ex is HttpRequestException or TaskCanceledException or JsonException or NotSupportedException;

}
