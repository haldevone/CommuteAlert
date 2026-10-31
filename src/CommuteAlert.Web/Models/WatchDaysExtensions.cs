namespace CommuteAlert.Web.Models;

public static class WatchDaysExtensions
{
    private static readonly WatchDays[] SingleDays =
    {
        WatchDays.Monday, WatchDays.Tuesday, WatchDays.Wednesday, WatchDays.Thursday,
        WatchDays.Friday, WatchDays.Saturday, WatchDays.Sunday
    };

    public static string SwedishName(this WatchDays day) => day switch
    {
        WatchDays.Monday => "Måndag",
        WatchDays.Tuesday => "Tisdag",
        WatchDays.Wednesday => "Onsdag",
        WatchDays.Thursday => "Torsdag",
        WatchDays.Friday => "Fredag",
        WatchDays.Saturday => "Lördag",
        WatchDays.Sunday => "Söndag",
        _ => day.ToString()
    };

    public static string ToSwedish(this WatchDays days)
    {
        if (days == WatchDays.Weekdays) return "Vardagar";

        if (SingleDays.All(d => days.HasFlag(d))) return "Alla dagar";

        return string.Join(", ", SingleDays
            .Where(d => days.HasFlag(d))
            .Select(d => d.SwedishName()[..3].ToLower()));
    }
}