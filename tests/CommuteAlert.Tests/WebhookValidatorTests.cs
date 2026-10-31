using CommuteAlert.Web.Security;

namespace CommuteAlert.Tests;

public class WebhookValidatorTests
{
    [Theory] // T-12
    [InlineData("https://discord.com/api/webhooks/123456/abc-DEF_token")]
    public void Valid_discord_address_is_accepted(string url)
        => Assert.True(WebhookValidator.IsValid(url));

    [Theory] // T-13
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a url")]
    [InlineData("http://discord.com/api/webhooks/1/a")]
    [InlineData("https://evil.com/api/webhooks/1/a")]
    [InlineData("https://discord.com.evil.com/api/webhooks/1/a")]
    [InlineData("https://discord.com@evil.com/api/webhooks/1/a")]
    [InlineData("https://discord.com:8443/api/webhooks/1/a")]
    [InlineData("https://discord.com/other/path")]
    [InlineData("https://discord.com/api/webhooks/1/a?x=1")]
    public void Other_addresses_are_rejected(string? url)
        => Assert.False(WebhookValidator.IsValid(url));
}