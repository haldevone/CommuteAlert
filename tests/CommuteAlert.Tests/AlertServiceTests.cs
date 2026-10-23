using CommuteAlert.Web.Data;
using CommuteAlert.Web.Models;
using CommuteAlert.Web.Services;
using CommuteAlert.Web.Sl.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CommuteAlert.Tests;

public class AlertServiceTests
{
    private static readonly DateTime InsideWindow = new(2026, 10, 14, 7, 30, 0);
    private static readonly DateTime OutsideWindow = new(2026, 10, 14, 12, 0, 0);

    private static ApplicationDbContext CreateDb()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection).Options;
        var db = new ApplicationDbContext(options);
        db.Database.EnsureCreated();
        db.Users.Add(new IdentityUser { Id = "user1", UserName = "test" });
        db.SaveChanges();
        return db;
    }

    private static void AddWatch(ApplicationDbContext db)
    {
        db.Watches.Add(new Watch
        {
            UserId = "user1",
            SiteId = 1,
            LineId = 13,
            DirectionCode = 2,
            Days = WatchDays.Weekdays,
            StartTime = new TimeOnly(7, 0),
            EndTime = new TimeOnly(9, 0)
        });
        db.SaveChanges();
    }

    private static SlDeparture CancelledDeparture()
    {
        var s = new DateTime(2026, 10, 14, 8, 0, 0);
        return new SlDeparture("Norsborg", 2, "CANCELLED", s, s,
            new SlJourney(1, "NORMALPROGRESS", null),
            new SlStopArea(2601, "Liljeholmen", "METROSTN"),
            new SlStopPoint(2604, "Liljeholmen", "5"),
            new SlLine(13, "13", "METRO", null));
    }

    [Fact] // T-08
    public async Task Same_event_in_two_rounds_gives_one_alert()
    {
        using var db = CreateDb();
        AddWatch(db);
        var fake = new FakeSlApiService { Departures = { CancelledDeparture() } };
        var service = new AlertService(db, fake);

        var first = await service.CheckAndGetAlerts(InsideWindow);
        var second = await service.CheckAndGetAlerts(InsideWindow);

        Assert.Single(first);
        Assert.Empty(second);
        Assert.Equal(1, db.Alerts.Count());
    }

    [Fact] // T-09
    public async Task Round_outside_time_window_does_not_call_sl()
    {
        using var db = CreateDb();
        AddWatch(db);
        var fake = new FakeSlApiService { Departures = { CancelledDeparture() } };
        var service = new AlertService(db, fake);

        var result = await service.CheckAndGetAlerts(OutsideWindow);

        Assert.Empty(result);
        Assert.Empty(fake.DepartureCalls);
    }


}