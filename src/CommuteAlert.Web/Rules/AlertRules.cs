using CommuteAlert.Web.Models;
using CommuteAlert.Web.Sl.Models;
using CommuteAlert.Web.Sl;

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

            var what = $"{SlModeNames.Singular(d.Line.TransportMode)} {d.Line.Designation} till {d.Destination} ({d.Scheduled:HH:mm})";
            var key = $"{d.Journey.Id}:{d.Scheduled:yyyyMMddHHmm}";

            //Om avgång är inställd, lägg avisering i listan och fortsätt till nästa avgång
            if (d.State == "CANCELLED" || d.Journey.State == "CANCELLED")
            {
                result.Add(new(AlertType.Cancelled, $"Cancelled:{key}", $"{what} är inställd. Från {Where(d)}."));
                continue;
            }

            //Om avgång är försenad, räkna fram försenade minuter och lägg avisering i listan
            if (d.Expected is { } expected && expected - d.Scheduled >= TimeSpan.FromMinutes(watch.DelayThresholdMinutes))
            {
                var minutes = (int)(expected - d.Scheduled).TotalMinutes;
                result.Add(new(AlertType.Delayed, $"Delayed:{key}", $"{what} är försenad {minutes} min. Ny tid {expected:HH:mm}, från {Where(d)}."));
            }
        }

        //Kontrollerar om den aktuella tiden ligger inom bevakningsfönstret och om det finns några
        //aktiva trafikstörningar som påverkar bevakningen
        if (InWindow(watch, nowLocal))
        {
            foreach (var dev in deviations.Where(dev => IsActive(dev, nowLocal) && Affects(watch, dev)))
            {
                var variant = dev.MessageVariants.FirstOrDefault(v => v.Language == "sv")
              ?? dev.MessageVariants.FirstOrDefault();

                result.Add(new(AlertType.Disruption, $"Disruption:{dev.DeviationCaseId}", DisruptionText(variant)));
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

    // Returnerar en text som beskriver var avgången sker, inklusive station och spår/läge.
    private static string Where(SlDeparture d)
    {
        var designation = d.StopPoint.Designation;
        if (string.IsNullOrWhiteSpace(designation)) return d.StopArea.Name;

        var word = d.Line.TransportMode == "BUS" ? "läge" : "spår";
        return $"{d.StopArea.Name}, {word} {designation}";
    }

    // Skapar en text som beskriver trafikstörningen, inklusive rubrik och detaljer, och begränsar längden till 480 tecken.
    private static string DisruptionText(SlMessageVariant? variant)
    {
        if (variant == null) return "Störning på din linje.";

        var text = string.IsNullOrWhiteSpace(variant.ScopeAlias)
            ? "Störning: "
            : $"Störning ({variant.ScopeAlias}): ";
        text += variant.Header;

        var details = variant.Details?.Replace("\r", " ").Replace("\n", " ").Trim();
        if (!string.IsNullOrEmpty(details))
            text += $". {details}";

        return Truncate(text, 480);
    }

    //Behövs för vi vill inte spara > 500 tecken i databasen, men vill visa hela texten i aviseringen
    private static string Truncate(string text, int max) =>
    text.Length <= max ? text : text[..(max - 1)] + "…";

}