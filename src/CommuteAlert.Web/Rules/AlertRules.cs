using CommuteAlert.Web.Models;
using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Web.Rules;

public static class AlertRules
{
    public static List<AlertSuggestion> Evaluate(
        Watch watch, List<SlDeparture> departures, List<SlDeviation> deviations, DateTime nowLocal)
    {
        var result = new List<AlertSuggestion>();

        foreach (var d in departures.Where(d => Matches(watch, d) && InWindow(watch, d.Scheduled)))
        {
            if (d.State == "NOTEXPECTED" || d.Journey.State == "NOTEXPECTED")
                continue;

            var what = $"Line {d.Line.Designation} to {d.Destination} ({d.Scheduled:HH:mm})";
            var key = $"{d.Journey.Id}:{d.Scheduled:yyyyMMddHHmm}";

            if (d.State == "CANCELLED" || d.Journey.State == "CANCELLED")
            {
                result.Add(new(AlertType.Cancelled, $"Cancelled:{key}", $"{what} is cancelled"));
                continue;
            }

            if (d.Expected is { } expected
                && expected - d.Scheduled >= TimeSpan.FromMinutes(watch.DelayThresholdMinutes))
            {
                var minutes = (int)(expected - d.Scheduled).TotalMinutes;
                result.Add(new(AlertType.Delayed, $"Delayed:{key}", $"{what} is delayed {minutes} min"));
            }
        }

        if (InWindow(watch, nowLocal))
        {
            foreach (var dev in deviations.Where(dev => IsActive(dev, nowLocal) && Affects(watch, dev)))
            {
                var header = dev.MessageVariants.FirstOrDefault()?.Header ?? "Disruption";
                result.Add(new(AlertType.Disruption, $"Disruption:{dev.DeviationCaseId}", $"Disruption: {header}"));
            }
        }

        return result;
    }

    private static bool Matches(Watch w, SlDeparture d) =>
        (w.TransportMode == null || w.TransportMode == d.Line.TransportMode) &&
        (w.LineId == null || w.LineId == d.Line.Id) &&
        (w.DirectionCode == null || w.DirectionCode == d.DirectionCode);

    public static bool InWindow(Watch w, DateTime time)
    {
        var day = (WatchDays)(1 << (int)time.DayOfWeek);
        
        if (!w.Days.HasFlag(day)) return false;

        var t = TimeOnly.FromDateTime(time);

        return t >= w.StartTime && t <= w.EndTime;
    }

    // Kontrollerar om den aktuella tiden ligger mellan trafikstörningens publiceringsstart (From) och sluttid (Upto).
    private static bool IsActive(SlDeviation dev, DateTime nowLocal) =>
        nowLocal >= dev.Publish.From.DateTime &&
        nowLocal <= dev.Publish.Upto.DateTime;

    private static bool Affects(Watch w, SlDeviation dev)
    {
        var lines = dev.Scope?.Lines ?? new List<SlScopeLine>();

        if (w.LineId != null) return lines.Any(l => l.Id == w.LineId);
        if (w.TransportMode != null) return lines.Any(l => l.TransportMode == w.TransportMode);

        return false;
    }
}