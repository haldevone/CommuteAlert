using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Web.Sl;

public class SlApiService : ISlApiService
{
    private const string TransportUrl = "https://transport.integration.sl.se/v1";
    private const string DeviationsUrl = "https://deviations.integration.sl.se/v1";

    private readonly HttpClient _http;

    public SlApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SlDeparture>> GetDeparturesAsync(int siteId, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<SlDeparturesResponse>(
            $"{TransportUrl}/sites/{siteId}/departures", SlJson.Options, ct);

        return result?.Departures ?? new List<SlDeparture>();
    }

    public async Task<List<SlDeviation>> GetDeviationsAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<SlDeviation>>(
            $"{DeviationsUrl}/messages", SlJson.Options, ct);

        return result ?? new List<SlDeviation>();
    }

}
