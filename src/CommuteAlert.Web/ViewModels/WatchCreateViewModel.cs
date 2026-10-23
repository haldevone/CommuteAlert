using CommuteAlert.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CommuteAlert.Web.ViewModels;

public class WatchCreateViewModel
{
    public int SiteId { get; set; }
    public string? SiteName { get; set; }
    public string Target { get; set; } = "all";

    [ValidateNever]
    public List<SelectListItem> TargetOptions { get; set; } = new List<SelectListItem>();

    public List<WatchDays> SelectedDays { get; set; } = new List<WatchDays>();
    public TimeOnly StartTime { get; set; } = new TimeOnly(7, 0);
    public TimeOnly EndTime { get; set; } = new TimeOnly(9, 0);

    [Range(1, 120)]
    public int DelayThresholdMinutes { get; set; } = 5;

}
