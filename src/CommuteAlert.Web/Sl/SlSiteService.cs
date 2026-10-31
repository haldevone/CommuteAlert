using CommuteAlert.Web.Sl.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CommuteAlert.Web.Sl;

public class SlSiteService
{
    private const string Url = "https://transport.integration.sl.se/v1/sites";
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public SlSiteService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    //Hämta alla SL stationer, använd cachade 24h för att slippa hämta från sl hela tiden, annars hämta från SL..
    public async Task<List<SlSite>> GetAllSites(CancellationToken ct = default)
    {
        var sites = await _cache.GetOrCreateAsync("sl-sites", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            var result = await _http.GetFromJsonAsync<List<SlSite>>(Url, SlJson.Options, ct);
            return result ?? new List<SlSite>();
        });

        return sites ?? new List<SlSite>();
    }

    //Sök funktion för hållplatserna
    public async Task<List<SlSite>> SearchSites(string query, int max = 20, CancellationToken ct = default)
    {
        query = query.Trim();
        var all = await GetAllSites(ct);
        return all
            .Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(s => s.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            .ThenBy(s => s.Name)
            .Take(max)
            .ToList();
    }

    //Hämta hållplats med ID
    public async Task<SlSite?> GetSiteById(int id, CancellationToken ct = default)
    {
        return (await GetAllSites(ct)).FirstOrDefault(s => s.Id == id);
    }


}
