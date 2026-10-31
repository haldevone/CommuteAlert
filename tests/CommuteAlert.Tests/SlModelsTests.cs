using CommuteAlert.Web.Sl;
using CommuteAlert.Web.Sl.Models;
using System.Text.Json;

namespace CommuteAlert.Tests;

public class SlModelsTests
{

    [Fact] //T-10
    public void Departures_file_can_be_read()
    {
        var json = File.ReadAllText("TestData/departures.json");
        var result = JsonSerializer.Deserialize<SlDeparturesResponse>(json, SlJson.Options);

        Assert.NotNull(result);
        var first = result.Departures[0];

        Assert.Equal("Norsborg", first.Destination);
        Assert.Equal("METRO", first.Line.TransportMode);
        Assert.Equal(new DateTime(2026, 10, 14, 20, 55, 0), first.Scheduled);
    }

    [Fact] //T-11
    public void Deviations_file_can_be_read()
    {
        var json = File.ReadAllText("TestData/deviations.json");
        var result = JsonSerializer.Deserialize<List<SlDeviation>>(json, SlJson.Options);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
