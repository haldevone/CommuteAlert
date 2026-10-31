namespace CommuteAlert.Web.Notifications;

public class DiscordNotifier : INotifier
{

    private readonly HttpClient _http;
    private readonly ILogger<DiscordNotifier> _logger;

    public DiscordNotifier(HttpClient http, ILogger<DiscordNotifier> logger)
    {
        _http = http;
        _logger = logger;
    }
    public async Task<bool> SendAsync(string webhookUrl, string message, CancellationToken ct = default)
    {
        try
        {
            var payload = new 
            { 
                content = message, 
                allowed_mentions = new 
                { 
                    parse = Array.Empty<string>() // Förbjud att pinga nån i kanalen
                } 
            };
            using var response = await _http.PostAsJsonAsync(webhookUrl, payload, ct);
            
            if (response.IsSuccessStatusCode) return true;

            _logger.LogWarning("Discord returned status {StatusCode}", (int)response.StatusCode);
            return false;

        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            _logger.LogWarning("Could not reach Discord: {Error}", ex.GetType().Name);
            return false;
        }
    }
}
