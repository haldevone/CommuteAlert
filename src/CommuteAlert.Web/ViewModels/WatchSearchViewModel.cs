using CommuteAlert.Web.Sl.Models;

namespace CommuteAlert.Web.ViewModels;

public class WatchSearchViewModel
{
    public string? Query { get; set; }
    public List<SlSite> Results { get; set; } = new List<SlSite>();
    public string? Error { get; set; }

}
