using System.ComponentModel.DataAnnotations;

namespace CommuteAlert.Web.ViewModels;

public class WebhookViewModel
{

    [Required]
    public string? Url { get; set; }
    public bool HasWebhook { get; set; }


}
