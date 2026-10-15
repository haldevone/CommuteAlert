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

    public async  Task<List<Alert>> CheckAndGetAlerts(DateTime nowLocal, CancellationToken ct = default)
    {
        var created = new List<Alert>();

        // InWindow cannot be translated to SQL, so the filter runs in memory
        var watches = (await _db.Watches.Where(w => w.IsActive).ToListAsync(ct))
            .Where(w => AlertRules.InWindow(w, nowLocal))
            .ToList();

        if (watches.Count == 0) return created;

        var deviations = await _sl.GetDeviationsAsync(ct);

        var departuresBySite = new Dictionary<int, List<SlDeparture>>();

        foreach (var siteId in watches.Select(w => w.SiteId).Distinct())
            departuresBySite[siteId] = await _sl.GetDeparturesAsync(siteId, ct);


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
                if (!sentKeys.Add(s.Key)) continue; // already saved, or repeated in this round

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
