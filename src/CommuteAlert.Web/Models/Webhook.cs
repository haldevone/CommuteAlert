namespace CommuteAlert.Web.Models;

public class Webhook
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string EncryptedUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
