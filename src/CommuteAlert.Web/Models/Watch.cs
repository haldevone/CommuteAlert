namespace CommuteAlert.Web.Models;

public class Watch
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public int SiteId { get; set; }
    public string? TransportMode { get; set; }
    public int? LineId { get; set; }
    public int? DirectionCode { get; set; }
    public WatchDays Days { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DelayThresholdMinutes { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public string SiteName { get; set; } = "";
    public string TargetName { get; set; } = "";
}
