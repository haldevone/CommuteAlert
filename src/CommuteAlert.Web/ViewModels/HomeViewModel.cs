namespace CommuteAlert.Web.ViewModels;

public class HomeViewModel
{
    public bool SignedIn { get; set; }
    public int ActiveWatches { get; set; }
    public int PausedWatches { get; set; }
    public bool HasWebhook { get; set; }
    public List<AlertHistoryItem> RecentAlerts { get; set; } = new();
}
