using CommuteAlert.Web.Sl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Tests;

public class SlModelsTests
{

    [Fact]
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

    [Fact]
    public void Deviations_file_can_be_read()
    {
        var json = File.ReadAllText("TestData/deviations.json");
        var result = JsonSerializer.Deserialize<List<SlDeviation>>(json, SlJson.Options);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
