using CommuteAlert.Web.Models;
using CommuteAlert.Web.Rules;
using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Tests;

public class AlertRulesTests
{
    private static readonly DateTime Now = new(2026, 10, 14, 7, 30, 0);

    private static Watch NewWatch() => new()
    {
        SiteId = 1,
        LineId = 13,
        DirectionCode = 2,
        Days = WatchDays.Weekdays,
        StartTime = new TimeOnly(7, 0),
        EndTime = new TimeOnly(9, 0),
        DelayThresholdMinutes = 5
    };

    private static SlDeparture Departure(string state = "EXPECTED", string journeyState = "NORMALPROGRESS",
        int delaySeconds = 0, int lineId = 13, int directionCode = 2, DateTime? scheduled = null)
    {
        var s = scheduled ?? new DateTime(2026, 10, 14, 8, 0, 0);
        return new SlDeparture("Norsborg", directionCode, state, s, s.AddSeconds(delaySeconds),
            new SlJourney(1, journeyState, null),
            new SlStopArea(2601, "Liljeholmen", "METROSTN"),
            new SlStopPoint(2604, "Liljeholmen", "5"),
            new SlLine(lineId, "13", "METRO", null));
    }

    private static SlDeviation Deviation(int lineId)
    {
        return new( 1,
        new SlPublish(new DateTimeOffset(2026, 10, 14, 0, 0, 0, TimeSpan.FromHours(2)),
                      new DateTimeOffset(2026, 10, 15, 0, 0, 0, TimeSpan.FromHours(2))),
        new List<SlMessageVariant> { new("Stopped", "details", null, "sv") },
        new SlScope(null, new List<SlScopeLine> { new(lineId, "13", "METRO", null) }));
    }

    private static List<AlertSuggestion> Run(params SlDeparture[] departures)
    {
        return AlertRules.Evaluate(
            NewWatch(), 
            departures.ToList(), 
            new List<SlDeviation>(), 
            Now);
    }
        

    [Fact] // T-01
    public void Cancelled_departure_gives_alert()
    {
        var result = Run(Departure(state: "CANCELLED"));
        Assert.Single(result);
        Assert.Equal(AlertType.Cancelled, result[0].Type);
    }

    [Fact] // T-02
    public void Delay_over_threshold_gives_alert()
    {
        var result = Run(Departure(delaySeconds: 8 * 60));
        Assert.Single(result);
        Assert.Equal(AlertType.Delayed, result[0].Type);
        Assert.Contains("8 min", result[0].Message);
    }

    [Fact] // T-03
    public void Delay_under_threshold_gives_no_alert()
        => Assert.Empty(Run(Departure(delaySeconds: 3 * 60)));

    [Fact] // T-04
    public void Departure_outside_time_window_gives_no_alert()
        => Assert.Empty(Run(Departure(state: "CANCELLED", scheduled: new DateTime(2026, 10, 14, 10, 0, 0))));

    [Fact] // T-05
    public void Departure_on_wrong_weekday_gives_no_alert()
        => Assert.Empty(Run(Departure(state: "CANCELLED", scheduled: new DateTime(2026, 10, 17, 8, 0, 0))));

    [Fact] // T-06
    public void Disruption_on_watched_line_gives_alert()
    {
        var result = AlertRules.Evaluate(NewWatch(), new(), new List<SlDeviation> { Deviation(13) }, Now);
        Assert.Single(result);
        Assert.Equal(AlertType.Disruption, result[0].Type);
    }

    [Fact] // T-07
    public void Disruption_on_other_line_gives_no_alert()
    {
        Assert.Empty(
            AlertRules.Evaluate(
                NewWatch(), 
                new(), 
                new List<SlDeviation> { Deviation(14) }, 
                Now));
    }

    [Fact] // T-08 
    public void Same_departure_gives_same_key_each_time()
    {
        var first = Run(Departure(state: "CANCELLED"));
        var second = Run(Departure(state: "CANCELLED"));
        Assert.Equal(first[0].Key, second[0].Key);
    }
}