using CommuteAlert.Web.Sl;
using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Tests;

public class FakeSlApiService : ISlApiService
{
    public List<SlDeparture> Departures { get; set; } = new();
    public List<SlDeviation> Deviations { get; set; } = new();
    public List<int> DepartureCalls { get; } = new();

    public Task<List<SlDeparture>> GetDeparturesAsync(int siteId, CancellationToken ct = default)
    {
        DepartureCalls.Add(siteId);
        return Task.FromResult(Departures);
    }

    public Task<List<SlDeviation>> GetDeviationsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Deviations);
    }

}
