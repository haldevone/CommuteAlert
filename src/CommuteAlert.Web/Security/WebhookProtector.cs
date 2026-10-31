using Microsoft.AspNetCore.DataProtection;

namespace CommuteAlert.Web.Security;

public class WebhookProtector
{
    private readonly IDataProtector _protector;

    public WebhookProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("CommuteAlert.Webhook.v1");
    }

    public string Protect(string url)
    {
        return _protector.Protect(url);
    }

    public string Unprotect(string protectedUrl)
    {
        return _protector.Unprotect(protectedUrl);
    }
}
