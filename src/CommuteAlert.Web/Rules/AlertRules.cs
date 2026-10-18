using CommuteAlert.Web.Models;
using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Web.Rules;

public static class AlertRules
{

    //Tar emot 4 parametrar, användarens bevakningsinställningar, avgångar från SL, Störningar från SL, aktuell tid
    //Retunerar en lista med aviseringar som matchar användarens bevakning och avgångar/störningar
    public static List<AlertSuggestion> Evaluate(
        Watch watch, List<SlDeparture> departures, List<SlDeviation> deviations, DateTime nowLocal)
    {
        // Sparar alla aviseringar i lista
        var result = new List<AlertSuggestion>();

        foreach (var d in departures.Where(d => Matches(watch, d) && InWindow(watch, d.Scheduled)))
        {
            if (d.State == "NOTEXPECTED" || d.Journey.State == "NOTEXPECTED")
                continue;

            var what = $"Line {d.Line.Designation} to {d.Destination} ({d.Scheduled:HH:mm})";
            var key = $"{d.Journey.Id}:{d.Scheduled:yyyyMMddHHmm}";

            //Om avgång är inställd, lägg avisering i listan och fortsätt till nästa avgång
            if (d.State == "CANCELLED" || d.Journey.State == "CANCELLED")
            {
                result.Add(new(AlertType.Cancelled, $"Cancelled:{key}", $"{what} is cancelled"));
                continue;
            }

            //Om avgång är försenad, räkna fram försenade minuter och lägg avisering i listan
            if (d.Expected is { } expected
                && expected - d.Scheduled >= TimeSpan.FromMinutes(watch.DelayThresholdMinutes))
            {
                var minutes = (int)(expected - d.Scheduled).TotalMinutes;
                result.Add(new(AlertType.Delayed, $"Delayed:{key}", $"{what} is delayed {minutes} min"));
            }
        }

        //Kontrollerar om den aktuella tiden ligger inom bevakningsfönstret och om det finns några
        //aktiva trafikstörningar som påverkar bevakningen
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

    //Kontrollerar om bevakningen matchar avgången baserat på transportmedel, linje och färdriktning
    private static bool Matches(Watch w, SlDeparture d) =>
        (w.TransportMode == null || w.TransportMode == d.Line.TransportMode) &&
        (w.LineId == null || w.LineId == d.Line.Id) &&
        (w.DirectionCode == null || w.DirectionCode == d.DirectionCode);

    //Är det en veckodag som användaren har valt att bevaka och ligger avgången inom bevakningstiden?
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

    // Kontrollerar om trafikstörningen påverkar bevakningen baserat på linje eller transportmedel.
    private static bool Affects(Watch w, SlDeviation dev)
    {
        var lines = dev.Scope?.Lines ?? new List<SlScopeLine>();

        if (w.LineId != null) return lines.Any(l => l.Id == w.LineId);
        if (w.TransportMode != null) return lines.Any(l => l.TransportMode == w.TransportMode);

        return false;
    }
}