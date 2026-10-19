namespace CommuteAlert.Web.Security;

public static class WebhookValidator
{
    public static bool IsValid(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        
        return uri.Scheme == Uri.UriSchemeHttps //Måste använda Https
            && uri.Host == "discord.com" //Måste vara Discord
            && uri.IsDefaultPort //Måste vara default port 443
            && string.IsNullOrEmpty(uri.UserInfo) //URL får inte innehålla användarnamn eller lösenord före domännamnet
            && string.IsNullOrEmpty(uri.Query) //URL får inte innehålla frågesträng
            && uri.AbsolutePath.StartsWith("/api/webhooks/"); //URL måste börja med /api/webhooks/
    }
}
