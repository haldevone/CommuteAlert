namespace CommuteAlert.Web.Sl;

public interface ISlApiService
{
    Task<List<SlDeparture>> GetDeparturesAsync(int siteId, CancellationToken ct = default);
    Task<List<SlDeviation>> GetDeviationsAsync(CancellationToken ct = default);
}
