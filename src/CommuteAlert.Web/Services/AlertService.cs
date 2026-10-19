using CommuteAlert.Web.Data;
using CommuteAlert.Web.Models;
using CommuteAlert.Web.Rules;
using CommuteAlert.Web.Sl;
using CommuteAlert.Web.Sl.Models;
using Microsoft.EntityFrameworkCore;

namespace CommuteAlert.Web.Services;

public class AlertService
{

    private readonly ApplicationDbContext _db;
    private readonly ISlApiService _sl;

    public AlertService(ApplicationDbContext db, ISlApiService sl)
    {
        _db = db;
        _sl = sl;
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

        var deviations = await _sl.GetDeviationsAsync(ct);

        var departuresBySite = new Dictionary<int, List<SlDeparture>>();

        // Hämta departures för varje unik siteId som finns i watches
        foreach (var siteId in watches.Select(w => w.SiteId).Distinct())
            departuresBySite[siteId] = await _sl.GetDeparturesAsync(siteId, ct);

        // Utvärdera varje watch mot departures och deviations, och skapa alerts om det behövs, undvik dubbletter
        foreach (var watch in watches)
        {
            var suggestions = AlertRules.Evaluate(
                watch, departuresBySite[watch.SiteId], deviations, nowLocal);

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


}
