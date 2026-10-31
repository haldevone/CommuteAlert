using CommuteAlert.Web.Models;

namespace CommuteAlert.Web.ViewModels;

public class AlertHistoryItem
{
    public DateTime CreatedAt { get; set; }
    public string SiteName { get; set; } = "";
    public AlertType Type { get; set; }
    public string Message { get; set; } = "";
    public bool Sent { get; set; }
}
