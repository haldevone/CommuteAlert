namespace CommuteAlert.Web.Notifications;

public interface INotifier
{
    Task<bool> SendAsync(string webhookUrl, string message, CancellationToken ct = default);
}
